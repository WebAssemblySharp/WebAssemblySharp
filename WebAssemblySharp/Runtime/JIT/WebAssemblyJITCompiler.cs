using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.MetaData.Utils;
using WebAssemblySharp.Runtime;
using WebAssemblySharp.Runtime.JIT;
using WebAssemblySharp.Runtime.Utils;

[RequiresDynamicCode("WebAssemblyJITCompiler requires dynamic code.")]
public class WebAssemblyJITCompiler
{
    protected readonly WasmMetaData m_WasmMetaData;
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private readonly Type m_ProxyType;
    protected Dictionary<String, MethodInfo> m_ExportedMethods;
    private Dictionary<WasmCode, MethodInfo> m_DynamicMethodsByCode;
    private List<FieldInfo> m_GlobalFields;
    protected List<FieldInfo> m_MemoryFields;
    protected List<FieldInfo> m_SyncExternalFunctionFields;
    protected List<FieldInfo> m_AsyncExternalFunctionFields;
    protected TypeBuilder m_TypeBuilder;

    private WasmFuncType m_CurrentFuncType;
    private LocalBuilder[] m_CurrentLocals;
    private List<Label> m_CurrentLabels;
    private Label m_ReturnLabel;

    public WebAssemblyJITCompiler(WasmMetaData p_WasmMetaData, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type p_ProxyType)
    {
        m_WasmMetaData = p_WasmMetaData;
        m_ProxyType = p_ProxyType;
    }

    protected void Reset()
    {
        m_TypeBuilder = null;
        m_DynamicMethodsByCode = null;
        m_ExportedMethods = null;
        m_GlobalFields = null;
        m_MemoryFields = null;
        m_SyncExternalFunctionFields = null;
        m_AsyncExternalFunctionFields = null;
    }

    public void Compile()
    {
        AssemblyName l_AssemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder l_AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(l_AssemblyName, AssemblyBuilderAccess.RunAndCollect);
        ModuleBuilder l_ModuleBuilder = l_AssemblyBuilder.DefineDynamicModule("DynamicModule");

        m_TypeBuilder = l_ModuleBuilder.DefineType(GetDynamicTypeName(), TypeAttributes.Public | TypeAttributes.Sealed);

        if (m_ProxyType != null)
        {
            m_TypeBuilder.AddInterfaceImplementation(m_ProxyType);
        }

        ApplyFields();
        ApplyConstructor();
        ApplyMethods();
    }

    private void ApplyMethods()
    {
        m_ExportedMethods = new Dictionary<string, MethodInfo>();
        m_DynamicMethodsByCode = new Dictionary<WasmCode, MethodInfo>();

        foreach (WasmExport l_Export in m_WasmMetaData.Export)
        {
            if (l_Export.Kind != WasmExternalKind.Function)
                continue;

            // Find the function index in the module
            int? l_FunctionIndex = WasmMetaDataUtils.FindExportIndex(m_WasmMetaData, l_Export.Name.Value, l_Export.Kind);

            if (l_FunctionIndex == null)
                throw new Exception($"Export not found: {l_Export.Name.Value}");

            // Get the function signature
            long l_FinalIndex = m_WasmMetaData.FuncIndex[l_FunctionIndex.Value];
            WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_FinalIndex];
            WasmCode l_Code = m_WasmMetaData.Code[l_FunctionIndex.Value];

            // Compile the function
            MethodInfo l_Method = CompileCode(m_TypeBuilder, l_Export.Name, l_FuncType, l_Code);
            m_ExportedMethods.Add(l_Export.Name, l_Method);
        }
    }

    private void ApplyFields()
    {
        // Create Globals
        m_GlobalFields = new List<FieldInfo>();
        
        if (m_WasmMetaData.Globals != null)
        {
            for (int i = 0; i < m_WasmMetaData.Globals.Length; i++)
            {
                WasmGlobal l_WasmGlobal = m_WasmMetaData.Globals[i];
                string l_FieldName = BuildGlobalFieldName(i);
                FieldBuilder l_FieldBuilder = m_TypeBuilder.DefineField(l_FieldName, WebAssemblyDataTypeUtils.GetInternalType(l_WasmGlobal.Type),
                    l_WasmGlobal.Mutable ? FieldAttributes.Private | FieldAttributes.InitOnly : FieldAttributes.Private);
                m_GlobalFields.Add(l_FieldBuilder);
            }
        }

        // Create Memory Areas
        m_MemoryFields = new List<FieldInfo>();
        
        if (m_WasmMetaData.Memory != null)
        {
            for (int i = 0; i < m_WasmMetaData.Memory.Length; i++)
            {
                string l_FieldName = BuildMemoryFieldName(i);
                FieldBuilder l_FieldBuilder = m_TypeBuilder.DefineField(l_FieldName, typeof(byte[]), FieldAttributes.Public);
                m_MemoryFields.Add(l_FieldBuilder);
            }
        }

        if (m_WasmMetaData.Import != null)
        {
            foreach (WasmImport l_WasmImport in m_WasmMetaData.Import)
            {
                if (l_WasmImport.Kind != WasmExternalKind.Memory)
                    continue;
                
                string l_FieldName = BuildMemoryFieldName(m_MemoryFields.Count);
                FieldBuilder l_FieldBuilder = m_TypeBuilder.DefineField(l_FieldName, typeof(byte[]), FieldAttributes.Public);
                m_MemoryFields.Add(l_FieldBuilder);
            }
        }

        // Create Imported Methods
        
        m_SyncExternalFunctionFields = new List<FieldInfo>();
        m_AsyncExternalFunctionFields = new List<FieldInfo>();
        
        if (m_WasmMetaData.Import != null)
        {
            foreach (WasmImport l_WasmImport in m_WasmMetaData.Import)
            {
                if (l_WasmImport.Kind != WasmExternalKind.Function)
                    continue;
                
                WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[((WasmImportFunction)l_WasmImport).FunctionIndex];
                
                string l_FieldName = BuildAsyncExternalFunctionFieldName(m_AsyncExternalFunctionFields.Count);
                Type l_FieldType = WebAssemblyJITCompilerUtils.GetAsyncFuncType(l_FuncType);
                FieldBuilder l_FieldBuilder = m_TypeBuilder.DefineField(l_FieldName, l_FieldType, FieldAttributes.Public);
                m_AsyncExternalFunctionFields.Add(l_FieldBuilder);
                
                l_FieldName = BuildSyncExternalFunctionFieldName(m_SyncExternalFunctionFields.Count);
                l_FieldType = WebAssemblyJITCompilerUtils.GetSyncFuncType(l_FuncType);
                l_FieldBuilder = m_TypeBuilder.DefineField(l_FieldName, l_FieldType, FieldAttributes.Public);
                m_SyncExternalFunctionFields.Add(l_FieldBuilder);
            }
        }
    }

    private String BuildMemoryFieldName(int p_Index)
    {
        return "memory_" + p_Index;
    }
    
    private String BuildAsyncExternalFunctionFieldName(int p_Index)
    {
        return "externalFunctionAsync_" + p_Index;
    }
    
    private String BuildSyncExternalFunctionFieldName(int p_Index)
    {
        return "externalFunction_" + p_Index;
    }
    
    private FieldInfo GetMemoryField(int p_Index)
    {
        return m_MemoryFields[p_Index];
    }
    
    private FieldInfo GetSyncExternalFunctionField(int p_Index)
    {
        return m_SyncExternalFunctionFields[p_Index];
    }
    
    private FieldInfo GetAsyncExternalFunctionField(int p_Index)
    {
        return m_AsyncExternalFunctionFields[p_Index];
    }
    
    private String BuildGlobalFieldName(int p_Index)
    {
        return "global_" + p_Index;
    }

    private FieldInfo GetGlobalField(int p_Index)
    {
        return m_GlobalFields[p_Index];
    }

    private void ApplyConstructor()
    {
        ILGenerator l_IlGenerator = m_TypeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new Type[] {}).GetILGenerator();
        l_IlGenerator.Emit(OpCodes.Ldarg_0);

        // Call the base class constructor, Object's constructor in this case
        ConstructorInfo l_ObjectCtor = typeof(object).GetConstructor(new Type[0]);
        l_IlGenerator.Emit(OpCodes.Call, l_ObjectCtor);

        // Initialize memory areas
        if (m_WasmMetaData.Memory != null)
        {
            for (int i = 0; i < m_WasmMetaData.Memory.Length; i++)
            {
                WasmMemory l_WasmMemory = m_WasmMetaData.Memory[i];

                // Create a new memory area
                Type l_MemoryType = typeof(byte[]);
                ConstructorInfo l_MemoryCtor = l_MemoryType.GetConstructor(new Type[] { typeof(int) });
                int l_Size = (int)(l_WasmMemory.Min * WebAssemblyConst.WASM_MEMORY_PAGE_SIZE);
                
                // Load "this"
                l_IlGenerator.Emit(OpCodes.Ldarg_0);
                
                // Create the memory area instance
                l_IlGenerator.Emit(OpCodes.Ldc_I4, l_Size);
                l_IlGenerator.Emit(OpCodes.Newobj, l_MemoryCtor);
                
                // Store the memory area in the field
                l_IlGenerator.Emit(OpCodes.Stfld, GetMemoryField(i));
            }
        }
        
        // Preload Memory Areas
        if (m_WasmMetaData.Data != null)
        {
            foreach (WasmData l_WasmData in m_WasmMetaData.Data)
            {

                FieldInfo l_MemoryField = GetMemoryField((int)l_WasmData.MemoryIndex);
                
                // Create a local variable to hold the value
                LocalBuilder l_OffsetLocalBuilder = l_IlGenerator.DeclareLocal(typeof(int));
                
                // Emit the initialization instructions to set the offset
                EmitInstructions(l_IlGenerator, l_WasmData.OffsetInstructions);
                
                // Store the result in the local variable
                l_IlGenerator.Emit(OpCodes.Stloc, l_OffsetLocalBuilder);

                byte[] l_Bytes = l_WasmData.Data.Data;

                for (int i = 0; i < l_Bytes.Length; i++)
                {
                    // Load the memory area
                    l_IlGenerator.Emit(OpCodes.Ldarg_0);
                    l_IlGenerator.Emit(OpCodes.Ldfld, l_MemoryField);
                    // Load the offset
                    l_IlGenerator.Emit(OpCodes.Ldloc, l_OffsetLocalBuilder);

                    if (i > 0)
                    {
                        l_IlGenerator.Emit(OpCodes.Ldc_I4, i);
                        l_IlGenerator.Emit(OpCodes.Add); // Add the offset to the index    
                    }
                    
                    // Load the byte value
                    l_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Bytes[i]);
                    l_IlGenerator.Emit(OpCodes.Conv_U1); // Convert the value to byte (unsigned)
                    // Store the byte value in the memory area
                    l_IlGenerator.Emit(OpCodes.Stelem_I1);
                }
            }
        }
        
        // Initialize globals
        if (m_WasmMetaData.Globals != null)
        {
            for (int i = 0; i < m_WasmMetaData.Globals.Length; i++)
            {
                WasmGlobal l_WasmGlobal = m_WasmMetaData.Globals[i];

                if (l_WasmGlobal.InitInstructions == null || !l_WasmGlobal.InitInstructions.Any())
                {
                    continue;
                }

                // Create a local variable to hold the value
                Type l_GlobalType = WebAssemblyDataTypeUtils.GetInternalType(l_WasmGlobal.Type);
                LocalBuilder l_LocalBuilder = l_IlGenerator.DeclareLocal(l_GlobalType);

                // Emit the initialization instructions
                EmitInstructions(l_IlGenerator, l_WasmGlobal.InitInstructions);

                // Store the result in the local variable
                l_IlGenerator.Emit(OpCodes.Stloc, l_LocalBuilder);

                // Load "this"
                l_IlGenerator.Emit(OpCodes.Ldarg_0);
                // Load the local variable onto the stack
                l_IlGenerator.Emit(OpCodes.Ldloc, l_LocalBuilder);
                // Store the value in the global field
                l_IlGenerator.Emit(OpCodes.Stfld, GetGlobalField(i));
            }
        }

        // Complete the constructor
        l_IlGenerator.Emit(OpCodes.Ret);
    }

    protected virtual string GetDynamicTypeName()
    {
        return "DynamicAssembly";
    }

    private MethodInfo CompileCode(TypeBuilder p_TypeBuilder, string p_ExportName, WasmFuncType p_FuncType, WasmCode p_Code)
    {
        if (m_ExportedMethods.TryGetValue(p_ExportName, out var l_Method))
            return l_Method;

        if (m_DynamicMethodsByCode.TryGetValue(p_Code, out var l_DynamicMethod))
            return l_DynamicMethod;

        Type l_ReturnType;

        if (p_FuncType.Results.Length == 0)
        {
            l_ReturnType = typeof(ValueTask);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ReturnType = typeof(ValueTask<>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]));
        }
        else
        {
            l_ReturnType = typeof(ValueTask<>).MakeGenericType(WebAssemblyValueTupleUtils.GetValueTupleType(p_FuncType.Results));
        }

        MethodBuilder l_MethodBuilder = p_TypeBuilder.DefineMethod(p_ExportName, MethodAttributes.Public | MethodAttributes.Virtual, l_ReturnType,
            p_FuncType.Parameters.Select(x => WebAssemblyDataTypeUtils.GetInternalType(x)).ToArray());
        m_DynamicMethodsByCode.Add(p_Code, l_MethodBuilder);
        ILGenerator l_IlGenerator = l_MethodBuilder.GetILGenerator();


        m_CurrentFuncType = p_FuncType;
        m_CurrentLocals = new LocalBuilder[p_Code.Locals != null ? p_Code.Locals.Length : 0];
        m_CurrentLabels = new List<Label>();

        if (p_Code.Locals != null)
        {
            // Define local variables
            for (int i = 0; i < p_Code.Locals.Length; i++)
            {
                Type l_LocalType = WebAssemblyDataTypeUtils.GetInternalType(p_Code.Locals[i]);
                LocalBuilder l_LocalBuilder = l_IlGenerator.DeclareLocal(l_LocalType);
                m_CurrentLocals[i] = l_LocalBuilder;
            }
        }

        m_ReturnLabel = l_IlGenerator.DefineLabel();

        if (IsRealAsync(p_Code.Instructions))
        {
            throw new Exception("Async functions are not supported yet");
        }
        else
        {
            DoCompileSyncMethod(l_IlGenerator, p_FuncType, p_Code);
        }

        l_IlGenerator.Emit(OpCodes.Ret);

        m_CurrentLocals = null;
        m_CurrentFuncType = null;

        if (m_CurrentLabels.Count != 0)
        {
            throw new Exception("Labels not used " + m_CurrentLabels.Count);
        }

        m_CurrentLabels = null;
        m_ReturnLabel = default(Label);

        return l_MethodBuilder;
    }

    private void DoCompileSyncMethod(ILGenerator p_IlGenerator, WasmFuncType p_FuncType, WasmCode p_Code)
    {
        EmitInstructions(p_IlGenerator, p_Code.Instructions);

        p_IlGenerator.MarkLabel(m_ReturnLabel);

        if (p_FuncType.Results.Length == 0)
        {
            p_IlGenerator.Emit(OpCodes.Call, typeof(ValueTask).GetProperty(nameof(ValueTask.CompletedTask)).GetMethod);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            Type l_ResultType = WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]);

            MethodInfo l_FromResultMethod = typeof(ValueTask).GetMethod(nameof(ValueTask.FromResult))
                .MakeGenericMethod(l_ResultType);

            p_IlGenerator.Emit(OpCodes.Call, l_FromResultMethod);
        }
        else
        {
            Type l_ResultType = WebAssemblyValueTupleUtils.GetValueTupleType(p_FuncType.Results);
            p_IlGenerator.Emit(OpCodes.Newobj,
                l_ResultType.GetConstructor(p_FuncType.Results.Select(x => WebAssemblyDataTypeUtils.GetInternalType(x)).ToArray()));

            MethodInfo l_FromResultMethod = typeof(ValueTask).GetMethod(nameof(ValueTask.FromResult))
                .MakeGenericMethod(l_ResultType);

            p_IlGenerator.Emit(OpCodes.Call, l_FromResultMethod);
        }
    }

    private void EmitInstructions(ILGenerator p_IlGenerator, IEnumerable<WasmInstruction> p_CodeInstructions)
    {
        if (p_CodeInstructions == null)
            return;

        // Emit IL for each instruction in the WebAssembly function
        foreach (WasmInstruction l_Instruction in p_CodeInstructions)
        {
            EmitInstruction(p_IlGenerator, l_Instruction);
        }
    }

    private bool IsRealAsync(IEnumerable<WasmInstruction> p_Code)
    {
        if (p_Code == null)
            return false;

        foreach (WasmInstruction l_Instruction in p_Code)
        {
            if (l_Instruction is WasmCall)
            {
                WasmCall l_WasmCall = (WasmCall)l_Instruction;

                Delegate l_Delegate = GetImortMethod(l_WasmCall);
            }

            if (l_Instruction is WasmBlockInstruction)
            {
                WasmBlockInstruction l_BlockInstruction = (WasmBlockInstruction)l_Instruction;

                if (IsRealAsync(l_BlockInstruction.GetAllInstructions()))
                    return true;
            }
        }

        return false;
    }

    private Delegate GetImortMethod(WasmCall p_Instruction)
    {
        WasmImportFunction l_WasmImport = m_WasmMetaData.Import.Where(x => x is WasmImportFunction).Cast<WasmImportFunction>()
            .First(x => x.FunctionIndex == p_Instruction.FunctionIndex);

        if (l_WasmImport == null)
            throw new Exception($"Function not found: {p_Instruction.FunctionIndex}");

        return null;
    }

    private void EmitInstruction(ILGenerator p_IlGenerator, WasmInstruction p_Instruction)
    {
        // Map WebAssembly instructions to IL opcodes
        switch (p_Instruction.Opcode)
        {
            case WasmOpcode.Unreachable:
                p_IlGenerator.ThrowException(typeof(InvalidOperationException));
                return;
            case WasmOpcode.Nop:
                p_IlGenerator.Emit(OpCodes.Nop);
                return;
            case WasmOpcode.Block:
                CompileBlock(p_IlGenerator, (WasmBlock)p_Instruction);
                return;
            case WasmOpcode.Loop:
                CompileLoop(p_IlGenerator, (WasmLoop)p_Instruction);
                return;
            case WasmOpcode.If:
                CompileIf(p_IlGenerator, (WasmIf)p_Instruction);
                return;
            case WasmOpcode.Else:
                break;
            case WasmOpcode.End:
                break;
            case WasmOpcode.Br:
                CompileBr(p_IlGenerator, (WasmBr)p_Instruction);
                return;
            case WasmOpcode.BrIf:
                CompileBrIf(p_IlGenerator, (WasmBrIf)p_Instruction);
                return;
            case WasmOpcode.BrTable:
                break;
            case WasmOpcode.Return:
                // Emit a branch to the return label
                p_IlGenerator.Emit(OpCodes.Br, m_ReturnLabel);
                return;
            case WasmOpcode.Call:
                CompileCall(p_IlGenerator, (WasmCall)p_Instruction);
                return;
            case WasmOpcode.CallIndirect:
                break;
            case WasmOpcode.Drop:
                break;
            case WasmOpcode.Select:
                break;
            case WasmOpcode.LocalGet:
                CompileLocalGet(p_IlGenerator, (WasmLocalGet)p_Instruction);
                return;
            case WasmOpcode.LocalSet:
                CompileLocalSet(p_IlGenerator, (WasmLocalSet)p_Instruction);
                return;
            case WasmOpcode.LocalTee:
                break;
            case WasmOpcode.GlobalGet:
                CompileGlobalGet(p_IlGenerator, (WasmGlobalGet)p_Instruction);
                return;
            case WasmOpcode.GlobalSet:
                break;
            case WasmOpcode.I32Load:
                CompileI32Load(p_IlGenerator, (WasmI32Load)p_Instruction);
                return;
            case WasmOpcode.I64Load:
                break;
            case WasmOpcode.F32Load:
                break;
            case WasmOpcode.F64Load:
                break;
            case WasmOpcode.I32Load8S:
                break;
            case WasmOpcode.I32Load8U:
                CompileI32Load8U(p_IlGenerator, (WasmI32Load8U)p_Instruction);
                return;
            case WasmOpcode.I32Load16S:
                break;
            case WasmOpcode.I32Load16U:
                break;
            case WasmOpcode.I64Load8S:
                break;
            case WasmOpcode.I64Load8U:
                break;
            case WasmOpcode.I64Load16S:
                break;
            case WasmOpcode.I64Load16U:
                break;
            case WasmOpcode.I64Load32S:
                break;
            case WasmOpcode.I64Load32U:
                break;
            case WasmOpcode.I32Store:
                break;
            case WasmOpcode.F32Store:
                break;
            case WasmOpcode.F64Store:
                break;
            case WasmOpcode.I32Store8:
                CompileI32Store8(p_IlGenerator, (WasmI32Store8)p_Instruction);
                return;
            case WasmOpcode.I32Store16:
                break;
            case WasmOpcode.I64Store8:
                break;
            case WasmOpcode.I64Store16:
                break;
            case WasmOpcode.I64Store32:
                break;
            case WasmOpcode.MemorySize:
                CompileMemorySize(p_IlGenerator, (WasmMemorySize)p_Instruction);
                return;
            case WasmOpcode.MemoryGrow:
                CompileMemoryGrow(p_IlGenerator, (WasmMemoryGrow)p_Instruction);
                return;
            case WasmOpcode.I32Const:
                p_IlGenerator.Emit(OpCodes.Ldc_I4, ((WasmI32Const)p_Instruction).Const);
                return;
            case WasmOpcode.I64Const:
                break;
            case WasmOpcode.F32Const:
                break;
            case WasmOpcode.F64Const:
                break;
            case WasmOpcode.I32Eqz:
                p_IlGenerator.Emit(OpCodes.Ldc_I4_0); // Load 0
                p_IlGenerator.Emit(OpCodes.Ceq); // Compare equal
                return;
            case WasmOpcode.I32Eq:
                p_IlGenerator.Emit(OpCodes.Ceq);
                return;
            case WasmOpcode.I32Ne:
                // The following sequence is equivalent to:
                // (a != b) == !(a == b)
                p_IlGenerator.Emit(OpCodes.Ceq); // Compare equal
                p_IlGenerator.Emit(OpCodes.Ldc_I4_0); // Load 0
                p_IlGenerator.Emit(OpCodes.Ceq); // Compare equal
                return;
            case WasmOpcode.I32LtS:
                p_IlGenerator.Emit(OpCodes.Clt);
                return;
            case WasmOpcode.I32LtU:
                p_IlGenerator.Emit(OpCodes.Clt_Un);
                return;
            case WasmOpcode.I32GtS:
                p_IlGenerator.Emit(OpCodes.Cgt); // Lower than
                return;
            case WasmOpcode.I32GtU:
                break;
            case WasmOpcode.I32LeS:
                break;
            case WasmOpcode.I32LeU:
                break;
            case WasmOpcode.I32GeS:
                // The following sequence is equivalent to:
                // (a >= b) == !(b < a)
                p_IlGenerator.Emit(OpCodes.Clt); // Lower than
                p_IlGenerator.Emit(OpCodes.Ldc_I4_0); // Load 0
                p_IlGenerator.Emit(OpCodes.Ceq); // Compare equal
                return;
            case WasmOpcode.I32GeU:
                // The following sequence is equivalent to:
                // (a >= b) == !(b < a)
                p_IlGenerator.Emit(OpCodes.Clt_Un); // Lower than unsinged
                p_IlGenerator.Emit(OpCodes.Ldc_I4_0); // Load 0
                p_IlGenerator.Emit(OpCodes.Ceq); // Compare equal
                return;
            case WasmOpcode.I64Eqz:
                break;
            case WasmOpcode.I64Eq:
                break;
            case WasmOpcode.I64Ne:
                break;
            case WasmOpcode.I64LtS:
                break;
            case WasmOpcode.I64LtU:
                break;
            case WasmOpcode.I64GtS:
                break;
            case WasmOpcode.I64GtU:
                break;
            case WasmOpcode.I64LeS:
                break;
            case WasmOpcode.I64LeU:
                break;
            case WasmOpcode.I64GeS:
                break;
            case WasmOpcode.I64GeU:
                break;
            case WasmOpcode.F32Eq:
                break;
            case WasmOpcode.F32Ne:
                break;
            case WasmOpcode.F32Lt:
                break;
            case WasmOpcode.F32Gt:
                break;
            case WasmOpcode.F32Le:
                break;
            case WasmOpcode.F32Ge:
                break;
            case WasmOpcode.F64Eq:
                break;
            case WasmOpcode.F64Ne:
                break;
            case WasmOpcode.F64Lt:
                break;
            case WasmOpcode.F64Gt:
                break;
            case WasmOpcode.F64Le:
                break;
            case WasmOpcode.F64Ge:
                break;
            case WasmOpcode.I32Clz:
                break;
            case WasmOpcode.I32Ctz:
                break;
            case WasmOpcode.I32Popcnt:
                break;
            case WasmOpcode.I32Add:
                p_IlGenerator.Emit(OpCodes.Add);
                return;
            case WasmOpcode.I32Sub:
                p_IlGenerator.Emit(OpCodes.Sub);
                return;
            case WasmOpcode.I32Mul:
                p_IlGenerator.Emit(OpCodes.Mul);
                return;
            case WasmOpcode.I32DivS:
                break;
            case WasmOpcode.I32DivU:
                p_IlGenerator.Emit(OpCodes.Div_Un);
                return;
            case WasmOpcode.I32RemS:
                break;
            case WasmOpcode.I32RemU:
                p_IlGenerator.Emit(OpCodes.Rem_Un);
                return;
            case WasmOpcode.I32And:
                break;
            case WasmOpcode.I32Or:
                break;
            case WasmOpcode.I32Xor:
                break;
            case WasmOpcode.I32Shl:
                break;
            case WasmOpcode.I32ShrS:
                break;
            case WasmOpcode.I32ShrU:
                break;
            case WasmOpcode.I32Rotl:
                break;
            case WasmOpcode.I32Rotr:
                break;
            case WasmOpcode.I64Clz:
                break;
            case WasmOpcode.I64Ctz:
                break;
            case WasmOpcode.I64Popcnt:
                break;
            case WasmOpcode.I64Add:
                break;
            case WasmOpcode.I64Sub:
                break;
            case WasmOpcode.I64Mul:
                break;
            case WasmOpcode.I64DivS:
                break;
            case WasmOpcode.I64DivU:
                break;
            case WasmOpcode.I64RemS:
                break;
            case WasmOpcode.I64RemU:
                break;
            case WasmOpcode.I64And:
                break;
            case WasmOpcode.I64Or:
                break;
            case WasmOpcode.I64Xor:
                break;
            case WasmOpcode.I64Shl:
                break;
            case WasmOpcode.I64ShrS:
                break;
            case WasmOpcode.I64ShrU:
                break;
            case WasmOpcode.I64Rotl:
                break;
            case WasmOpcode.I64Rotr:
                break;
            case WasmOpcode.F32Abs:
                break;
            case WasmOpcode.F32Neg:
                break;
            case WasmOpcode.F32Ceil:
                break;
            case WasmOpcode.F32Floor:
                break;
            case WasmOpcode.F32Trunc:
                break;
            case WasmOpcode.F32Nearest:
                break;
            case WasmOpcode.F32Sqrt:
                break;
            case WasmOpcode.F32Add:
                break;
            case WasmOpcode.F32Sub:
                break;
            case WasmOpcode.F32Mul:
                break;
            case WasmOpcode.F32Div:
                break;
            case WasmOpcode.F32Min:
                break;
            case WasmOpcode.F32Max:
                break;
            case WasmOpcode.F32Copysign:
                break;
            case WasmOpcode.F64Abs:
                break;
            case WasmOpcode.F64Neg:
                break;
            case WasmOpcode.F64Ceil:
                break;
            case WasmOpcode.F64Floor:
                break;
            case WasmOpcode.F64Trunc:
                break;
            case WasmOpcode.F64Nearest:
                break;
            case WasmOpcode.F64Sqrt:
                break;
            case WasmOpcode.F64Add:
                break;
            case WasmOpcode.F64Sub:
                break;
            case WasmOpcode.F64Mul:
                break;
            case WasmOpcode.F64Div:
                break;
            case WasmOpcode.F64Min:
                break;
            case WasmOpcode.F64Max:
                break;
            case WasmOpcode.F64Copysign:
                break;
            case WasmOpcode.I32WrapI64:
                break;
            case WasmOpcode.I32TruncF32S:
                break;
            case WasmOpcode.I32TruncF32U:
                break;
            case WasmOpcode.I32TruncF64S:
                break;
            case WasmOpcode.I32TruncF64U:
                break;
            case WasmOpcode.I64ExtendI32S:
                break;
            case WasmOpcode.I64ExtendI32U:
                break;
            case WasmOpcode.I64TruncF32S:
                break;
            case WasmOpcode.I64TruncF32U:
                break;
            case WasmOpcode.I64TruncF64S:
                break;
            case WasmOpcode.I64TruncF64U:
                break;
            case WasmOpcode.F32ConvertI32S:
                break;
            case WasmOpcode.F32ConvertI32U:
                break;
            case WasmOpcode.F32ConvertI64S:
                break;
            case WasmOpcode.F32ConvertI64U:
                break;
            case WasmOpcode.F32DemoteF64:
                break;
            case WasmOpcode.F64ConvertI32S:
                break;
            case WasmOpcode.F64ConvertI32U:
                break;
            case WasmOpcode.F64ConvertI64S:
                break;
            case WasmOpcode.F64ConvertI64U:
                break;
            case WasmOpcode.F64PromoteF32:
                break;
            case WasmOpcode.I32ReinterpretF32:
                break;
            case WasmOpcode.I64ReinterpretF64:
                break;
            case WasmOpcode.F32ReinterpretI32:
                break;
            case WasmOpcode.F64ReinterpretI64:
                break;
            case WasmOpcode.I32Extend8_s:
                break;
            case WasmOpcode.I32Extend16_s:
                break;
            case WasmOpcode.I64Extend8_s:
                break;
            case WasmOpcode.I64Extend16_s:
                break;
            case WasmOpcode.I64Extend32_s:
                break;
            case WasmOpcode.I32TruncSatF32S:
                break;
            case WasmOpcode.I32TruncSatF32U:
                break;
            case WasmOpcode.I32TruncSatF64S:
                break;
            case WasmOpcode.I32TruncSatF64U:
                break;
            case WasmOpcode.I64TruncSatF32S:
                break;
            case WasmOpcode.I64TruncSatF32U:
                break;
            case WasmOpcode.I64TruncSatF64S:
                break;
            case WasmOpcode.I64TruncSatF64U:
                break;
            case WasmOpcode.MemoryInit:
                break;
            case WasmOpcode.DataDrop:
                break;
            case WasmOpcode.MemoryCopy:
                break;
            case WasmOpcode.MemoryFill:
                CompileMemoryFill(p_IlGenerator, (WasmMemoryFill)p_Instruction);
                return;
            case WasmOpcode.TableInit:
                break;
            case WasmOpcode.ElemDrop:
                break;
            case WasmOpcode.TableCopy:
                break;
            case WasmOpcode.TableGrow:
                break;
            case WasmOpcode.TableSize:
                break;
            case WasmOpcode.TableFill:
                break;
            default:
                throw new ArgumentOutOfRangeException(p_Instruction.Opcode.ToString(), p_Instruction, null);
        }

        throw new NotImplementedException("Instruction not implemented: " + p_Instruction.Opcode);
    }

    private void CompileCall(ILGenerator p_IlGenerator, WasmCall p_Instruction)
    {
        
        WasmImportFunction l_WasmImportFunction = WebAssemblyImportUtils.FindImportByFilter<WasmImportFunction>(m_WasmMetaData, (x) => x.FunctionIndex == p_Instruction.FunctionIndex);
        WasmFuncType l_FuncType = WebAssemblyImportUtils.GetFuncType(m_WasmMetaData, l_WasmImportFunction);

        List<LocalBuilder> l_Locals = new List<LocalBuilder>();
        
        for (int i = 0; i < l_FuncType.Parameters.Length; i++)
        {
            LocalBuilder l_Local = p_IlGenerator.DeclareLocal(WebAssemblyDataTypeUtils.GetInternalType(l_FuncType.Parameters[i]));
            l_Locals.Add(l_Local);
            p_IlGenerator.Emit(OpCodes.Stloc, l_Local);
        }
        
        FieldInfo l_FunctionField = GetSyncExternalFunctionField((int)p_Instruction.FunctionIndex);
        FieldInfo l_AsyncFunctionField = GetAsyncExternalFunctionField((int)p_Instruction.FunctionIndex);
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the 'this' parameter
        p_IlGenerator.Emit(OpCodes.Ldfld, l_FunctionField); // Load the function pointer from the 'this' parameter;
        
        // Check if Field is null
        p_IlGenerator.Emit(OpCodes.Ldnull);
        p_IlGenerator.Emit(OpCodes.Ceq);

        Label l_AsyncLabel = p_IlGenerator.DefineLabel();
        

        p_IlGenerator.Emit(OpCodes.Brtrue, l_AsyncLabel); // If the field is null, go to async
        
        // Sync way
        ////////////////////////////////////////
        
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the 'this' parameter
        p_IlGenerator.Emit(OpCodes.Ldfld, l_FunctionField); // Load the function pointer from the 'this' parameter;

        for (int i = l_FuncType.Parameters.Length - 1; i >= 0; i--)
        {
            p_IlGenerator.Emit(OpCodes.Ldloc, l_Locals[i]);    
        }
        
        p_IlGenerator.EmitCall(OpCodes.Callvirt, l_FunctionField.FieldType.GetMethod("Invoke"), null); // Call the function
        
        Label l_End = p_IlGenerator.DefineLabel();
        p_IlGenerator.Emit(OpCodes.Br, l_End); // End of the sync way
        
        // Async way
        //////////////////////////////////////////
        
        p_IlGenerator.MarkLabel(l_AsyncLabel); 
        
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the 'this' parameter
        p_IlGenerator.Emit(OpCodes.Ldfld, l_AsyncFunctionField); // Load the function pointer from the 'this' parameter;

        for (int i = l_FuncType.Parameters.Length - 1; i >= 0; i--)
        {
            p_IlGenerator.Emit(OpCodes.Ldloc, l_Locals[i]);    
        }
        
        p_IlGenerator.EmitCall(OpCodes.Callvirt, l_AsyncFunctionField.FieldType.GetMethod("Invoke"), null); // Call the function
        
        // At this point there is an TaskValue on the Stack

        
        // End
        p_IlGenerator.MarkLabel(l_End);

        if (l_FuncType.Results != null && l_FuncType.Results.Length > 1)
        {
            // At this point there is a value tuple on the stack and we need to load each value of the tuple on the stack
            Type l_ResultType = WebAssemblyValueTupleUtils.GetValueTupleType(l_FuncType.Results);

            // Store tuple in local variable
            LocalBuilder l_TupleLocal = p_IlGenerator.DeclareLocal(l_ResultType);
            p_IlGenerator.Emit(OpCodes.Stloc, l_TupleLocal);

            // Load each field from tuple onto stack
            for (int i = 0; i < l_FuncType.Results.Length; i++)
            {
                p_IlGenerator.Emit(OpCodes.Ldloc, l_TupleLocal);
                p_IlGenerator.Emit(OpCodes.Ldfld, l_ResultType.GetField($"Item{i + 1}"));
            }
        }
    }


    private void CompileMemoryFill(ILGenerator p_IlGenerator, WasmMemoryFill p_Instruction)
    {
        
        LocalBuilder l_Length = p_IlGenerator.DeclareLocal(typeof(int));
        p_IlGenerator.Emit(OpCodes.Stloc, l_Length); // Store the length in the local variable;
        
        LocalBuilder l_Value = p_IlGenerator.DeclareLocal(typeof(int));
        p_IlGenerator.Emit(OpCodes.Ldc_I4, 255); // Load the maximum value for byte (255)
        p_IlGenerator.Emit(OpCodes.And); // Ensure the value is within byte range
        p_IlGenerator.Emit(OpCodes.Stloc, l_Value); // Store the value in the local variable
        
        LocalBuilder l_Offset = p_IlGenerator.DeclareLocal(typeof(int));
        p_IlGenerator.Emit(OpCodes.Stloc, l_Offset); // Store the offset in the local variable;
        
        LocalBuilder l_Span = p_IlGenerator.DeclareLocal(typeof(Span<byte>));
        
        p_IlGenerator.Emit(OpCodes.Ldloca, l_Span); // Load the address of the span
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        FieldInfo l_MemoryField = GetMemoryField(p_Instruction.MemoryIndex);
        p_IlGenerator.Emit(OpCodes.Ldfld, l_MemoryField); // Load the memory field

        
        // Create a span for the memory array
        p_IlGenerator.Emit(OpCodes.Ldloc, l_Offset); // Load the offset from the local variable
        p_IlGenerator.Emit(OpCodes.Ldloc, l_Length); // Load the length from the local variable
        p_IlGenerator.Emit(OpCodes.Call, typeof(Span<byte>).GetConstructor(new[] { typeof(byte[]), typeof(int), typeof(int) })); // Create a span for the memory array
        
        
        p_IlGenerator.Emit(OpCodes.Ldloca, l_Span); // Load the address of the span
        p_IlGenerator.Emit(OpCodes.Ldloc, l_Value); // Load the value from the local variable
        p_IlGenerator.Emit(OpCodes.Conv_U1); // Convert the value to byte (unsigned)
        p_IlGenerator.Emit(OpCodes.Call, typeof(Span<byte>).GetMethod(nameof(Span<byte>.Fill))); // Call the Fill method on the span to fill the memory area with the value
        
        
    }

    private void CompileMemorySize(ILGenerator p_IlGenerator, WasmMemorySize p_Instruction)
    {
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        FieldInfo l_MemoryField = GetMemoryField(p_Instruction.MemoryIndex);
        p_IlGenerator.Emit(OpCodes.Ldfld, l_MemoryField); // Load the memory field
        p_IlGenerator.Emit(OpCodes.Ldlen); // Load the length of the memory array
        p_IlGenerator.Emit(OpCodes.Ldc_I4, WebAssemblyConst.WASM_MEMORY_PAGE_SIZE); // Load the page size constant
        p_IlGenerator.Emit(OpCodes.Div); // Divide the length by the page size to get the current number of pages
        
    }

    private void CompileMemoryGrow(ILGenerator p_IlGenerator, WasmMemoryGrow p_Instruction)
    {
        
        LocalBuilder l_PageToAdd = p_IlGenerator.DeclareLocal(typeof(long));
        p_IlGenerator.Emit(OpCodes.Conv_I8);
        p_IlGenerator.Emit(OpCodes.Stloc, l_PageToAdd); // Store the value in the local variable
        
        p_IlGenerator.Emit(OpCodes.Ldloc, l_PageToAdd); // Load the number of pages to add from the local variable
        p_IlGenerator.Emit(OpCodes.Ldc_I8, 0L);
        Label l_NegativePages = p_IlGenerator.DefineLabel();
        p_IlGenerator.Emit(OpCodes.Bge_S, l_NegativePages); // Branch if the number of pages is negative
        
        p_IlGenerator.Emit(OpCodes.Ldstr, "Memory grow must be non-negative");
        p_IlGenerator.Emit(OpCodes.Newobj, typeof(InvalidOperationException).GetConstructor(new[] { typeof(string) }));
        p_IlGenerator.Emit(OpCodes.Throw); // Throw an exception if the number of pages is negative
        
        p_IlGenerator.MarkLabel(l_NegativePages); // Mark the label for the negative pages

        FieldInfo l_MemoryField = GetMemoryField(p_Instruction.MemoryIndex);

        // Call Current pages
        LocalBuilder l_CurrentPages = p_IlGenerator.DeclareLocal(typeof(long));
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        p_IlGenerator.Emit(OpCodes.Ldfld, l_MemoryField); // Load the memory field
        p_IlGenerator.Emit(OpCodes.Ldlen); // Load the length of the memory array
        p_IlGenerator.Emit(OpCodes.Ldc_I4, WebAssemblyConst.WASM_MEMORY_PAGE_SIZE); // Load the page size constant
        p_IlGenerator.Emit(OpCodes.Div); // Divide the length by the page size to get the current number of pages
        p_IlGenerator.Emit(OpCodes.Conv_I8); // Convert the result to long
        p_IlGenerator.Emit(OpCodes.Stloc, l_CurrentPages); // Store the current pages in the local variable
        
        // Calculate the new size
        LocalBuilder l_TargetPages = p_IlGenerator.DeclareLocal(typeof(long));
        p_IlGenerator.Emit(OpCodes.Ldloc, l_CurrentPages); // Load the current pages from the local variable
        p_IlGenerator.Emit(OpCodes.Ldloc, l_PageToAdd); // Load the number of pages to add from the local variable
        p_IlGenerator.Emit(OpCodes.Add); // Add the current pages and the pages to add
        p_IlGenerator.Emit(OpCodes.Stloc, l_TargetPages); // Store the target pages in the local variable
        
        // Check if the target pages exceed the maximum allowed memory size
        p_IlGenerator.Emit(OpCodes.Ldloc, l_TargetPages); // Load the target pages from the local variable
        p_IlGenerator.Emit(OpCodes.Ldc_I8, m_WasmMetaData.Memory[p_Instruction.MemoryIndex].Max); // Load the maximum memory size constant
        Label l_ResizeMemory = p_IlGenerator.DefineLabel();
        p_IlGenerator.Emit(OpCodes.Ble_S, l_ResizeMemory); // Branch if the target pages are less than or equal to the maximum memory size
        p_IlGenerator.Emit(OpCodes.Ldc_I4, -1); // Load -1 to indicate failure
        Label l_Return = p_IlGenerator.DefineLabel();
        p_IlGenerator.Emit(OpCodes.Br_S, l_Return); // Branch to return
        p_IlGenerator.MarkLabel(l_ResizeMemory); // Mark the label for resizing memory
        // Resize the memory array
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        p_IlGenerator.Emit(OpCodes.Ldfld, l_MemoryField); // Load the memory field
        LocalBuilder l_OldMemory = p_IlGenerator.DeclareLocal(typeof(byte[]));
        p_IlGenerator.Emit(OpCodes.Stloc, l_OldMemory); // Store the old memory in the local variable
        p_IlGenerator.Emit(OpCodes.Ldarg_0);
        p_IlGenerator.Emit(OpCodes.Ldloc, l_TargetPages); // Load the target pages from the local variable
        p_IlGenerator.Emit(OpCodes.Ldc_I4, WebAssemblyConst.WASM_MEMORY_PAGE_SIZE); // Load the page size constant
        p_IlGenerator.Emit(OpCodes.Mul); // Multiply the target pages by the page size to get the new size
        p_IlGenerator.Emit(OpCodes.Conv_I); // Convert the Size to int
        p_IlGenerator.Emit(OpCodes.Newarr, typeof(byte)); // Create a new byte array with the new size// Load the instance (this)
        p_IlGenerator.Emit(OpCodes.Stfld, l_MemoryField);
        
        p_IlGenerator.Emit(OpCodes.Ldloc, l_OldMemory); // Load the old memory from the local variable
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        p_IlGenerator.Emit(OpCodes.Ldfld, l_MemoryField); // Load the new memory field
        p_IlGenerator.Emit(OpCodes.Ldloc, l_OldMemory);
        p_IlGenerator.Emit(OpCodes.Ldlen); // Load the length of the old memory array
        p_IlGenerator.Emit(OpCodes.Conv_I4); // Convert the length to int
        p_IlGenerator.Emit(OpCodes.Call, typeof(Array).GetMethod(nameof(Array.Copy), new[] { typeof(Array), typeof(Array), typeof(int) })); // Copy the old memory to the new memory
        
        
        p_IlGenerator.Emit(OpCodes.Ldloc, l_CurrentPages);
        p_IlGenerator.Emit(OpCodes.Conv_I4);
        
        p_IlGenerator.MarkLabel(l_Return);
    }

    private void CompileI32Store8(ILGenerator p_IlGenerator, WasmI32Store8 p_Instruction)
    {
        LocalBuilder l_Value = p_IlGenerator.DeclareLocal(typeof(byte));
        p_IlGenerator.Emit(OpCodes.Ldc_I4, 255); // Load the maximum value for byte (255)
        p_IlGenerator.Emit(OpCodes.And); // Ensure the value is within byte range
        p_IlGenerator.Emit(OpCodes.Conv_U1); // Convert the value to byte (unsigned)
        p_IlGenerator.Emit(OpCodes.Stloc, l_Value); // Store the value in the local variable
        
        long l_Offset = p_Instruction.Offset;
        if (l_Offset != 0)
        {
            p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset); // Load the offset
            p_IlGenerator.Emit(OpCodes.Add); // Add the offset to the address
        }

        LocalBuilder l_Address = p_IlGenerator.DeclareLocal(typeof(int));
        p_IlGenerator.Emit(OpCodes.Stloc, l_Address); // Store the address in the local variable

        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
        
        p_IlGenerator.Emit(OpCodes.Ldloc, l_Address); // Load the address from the local variable
        p_IlGenerator.Emit(OpCodes.Ldloc, l_Value); // Load the value from the local variable
        
        p_IlGenerator.Emit(OpCodes.Stelem_I1); // Store the value at the address
        
    }
    
    private void CompileI32Load(ILGenerator p_IlGenerator, WasmI32Load p_Instruction)
{
    LocalBuilder l_IndexLocal = p_IlGenerator.DeclareLocal(typeof(int));
    p_IlGenerator.Emit(OpCodes.Stloc, l_IndexLocal);
    p_IlGenerator.Emit(OpCodes.Ldloc, l_IndexLocal); // Load the index from the local variable
    
    long l_Offset = p_Instruction.Offset;
    if (l_Offset != 0)
    {
        p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset); // Load the offset
        p_IlGenerator.Emit(OpCodes.Add); // Add the offset to the address
    }

    // Check bounds - ensure we don't read beyond array bounds
    p_IlGenerator.Emit(OpCodes.Ldc_I4_3); // Load constant 3 (size of int32 - 1)
    p_IlGenerator.Emit(OpCodes.Add); // Add to check if index + 3 is within bounds
    
    p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
    p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
    p_IlGenerator.Emit(OpCodes.Ldlen); // Get array length
    p_IlGenerator.Emit(OpCodes.Conv_I4); // Convert to int32
    
    Label l_InBounds = p_IlGenerator.DefineLabel();
    p_IlGenerator.Emit(OpCodes.Blt, l_InBounds); // Branch if less than (within bounds)
    
    // Throw exception if out of bounds
    p_IlGenerator.Emit(OpCodes.Ldstr, "WebAssembly memory access out of bounds");
    p_IlGenerator.Emit(OpCodes.Newobj, typeof(IndexOutOfRangeException).GetConstructor(new[] { typeof(string) }));
    p_IlGenerator.Emit(OpCodes.Throw);
    
    p_IlGenerator.MarkLabel(l_InBounds);
    
    // Load the 32-bit integer from memory (little-endian)
    // We need to load 4 bytes and combine them into an int32
    LocalBuilder l_ResultLocal = p_IlGenerator.DeclareLocal(typeof(int));
    p_IlGenerator.Emit(OpCodes.Ldc_I4_0); // Initialize result to 0
    p_IlGenerator.Emit(OpCodes.Stloc, l_ResultLocal);
    
    // Load byte at index
    p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
    p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
    p_IlGenerator.Emit(OpCodes.Ldloc, l_IndexLocal); // Load index
    if (l_Offset != 0)
    {
        p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset);
        p_IlGenerator.Emit(OpCodes.Add);
    }
    p_IlGenerator.Emit(OpCodes.Ldelem_U1); // Load unsigned byte
    p_IlGenerator.Emit(OpCodes.Stloc, l_ResultLocal); // Store as result
    
    // Load byte at index + 1, shift left by 8, and OR with result
    p_IlGenerator.Emit(OpCodes.Ldloc, l_ResultLocal); // Load current result
    p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
    p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
    p_IlGenerator.Emit(OpCodes.Ldloc, l_IndexLocal); // Load index
    p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset + 1);
    p_IlGenerator.Emit(OpCodes.Add);
    p_IlGenerator.Emit(OpCodes.Ldelem_U1); // Load unsigned byte
    p_IlGenerator.Emit(OpCodes.Ldc_I4_8); // Shift left by 8
    p_IlGenerator.Emit(OpCodes.Shl);
    p_IlGenerator.Emit(OpCodes.Or); // OR with result
    p_IlGenerator.Emit(OpCodes.Stloc, l_ResultLocal);
    
    // Load byte at index + 2, shift left by 16, and OR with result
    p_IlGenerator.Emit(OpCodes.Ldloc, l_ResultLocal); // Load current result
    p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
    p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
    p_IlGenerator.Emit(OpCodes.Ldloc, l_IndexLocal); // Load index
    p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset + 2);
    p_IlGenerator.Emit(OpCodes.Add);
    p_IlGenerator.Emit(OpCodes.Ldelem_U1); // Load unsigned byte
    p_IlGenerator.Emit(OpCodes.Ldc_I4_S, 16); // Shift left by 16
    p_IlGenerator.Emit(OpCodes.Shl);
    p_IlGenerator.Emit(OpCodes.Or); // OR with result
    p_IlGenerator.Emit(OpCodes.Stloc, l_ResultLocal);
    
    // Load byte at index + 3, shift left by 24, and OR with result
    p_IlGenerator.Emit(OpCodes.Ldloc, l_ResultLocal); // Load current result
    p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
    p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
    p_IlGenerator.Emit(OpCodes.Ldloc, l_IndexLocal); // Load index
    p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset + 3);
    p_IlGenerator.Emit(OpCodes.Add);
    p_IlGenerator.Emit(OpCodes.Ldelem_U1); // Load unsigned byte
    p_IlGenerator.Emit(OpCodes.Ldc_I4_S, 24); // Shift left by 24
    p_IlGenerator.Emit(OpCodes.Shl);
    p_IlGenerator.Emit(OpCodes.Or); // OR with result
    
    // The final result is now on the stack as an int32
}

    private void CompileI32Load8U(ILGenerator p_IlGenerator, WasmI32Load8U p_Instruction)
    {
        LocalBuilder l_IndexLocal = p_IlGenerator.DeclareLocal(typeof(int));
        p_IlGenerator.Emit(OpCodes.Stloc, l_IndexLocal);
        
        // Load the address of the memory location
        p_IlGenerator.Emit(OpCodes.Ldarg_0); // Load the instance (this)
        p_IlGenerator.Emit(OpCodes.Ldfld, GetMemoryField(0)); // Load the memory field
        
        p_IlGenerator.Emit(OpCodes.Ldloc, l_IndexLocal); // Load the index from the local variable
        
        long l_Offset = p_Instruction.Offset;
        if (l_Offset != 0)
        {
            p_IlGenerator.Emit(OpCodes.Ldc_I4, (int)l_Offset); // Load the offset
            p_IlGenerator.Emit(OpCodes.Add); // Add the offset to the address
        }
        // Access the memory area by index 
        p_IlGenerator.Emit(OpCodes.Ldelem_U1); // Load the byte at the address
        
    }

    private void CompileGlobalGet(ILGenerator p_IlGenerator, WasmGlobalGet p_Instruction)
    {
        p_IlGenerator.Emit(OpCodes.Ldarg_0);
        p_IlGenerator.Emit(OpCodes.Ldfld, GetGlobalField((int)p_Instruction.GlobalIndex));
    }

    private void CompileBr(ILGenerator p_IlGenerator, WasmBr p_Instruction)
    {
        p_IlGenerator.Emit(OpCodes.Br, m_CurrentLabels[m_CurrentLabels.Count - 1 - (int)p_Instruction.LabelIndex]);
    }

    private void CompileBrIf(ILGenerator p_IlGenerator, WasmBrIf p_Instruction)
    {
        p_IlGenerator.Emit(OpCodes.Brtrue, m_CurrentLabels[m_CurrentLabels.Count - 1 - (int)p_Instruction.LabelIndex]);
    }

    private void CompileBlock(ILGenerator p_IlGenerator, WasmBlock p_Instruction)
    {
        // Define the labels for the block
        Label l_EndLabel = p_IlGenerator.DefineLabel();

        // Compile the block body
        m_CurrentLabels.Add(l_EndLabel);
        EmitInstructions(p_IlGenerator, p_Instruction.Body);
        m_CurrentLabels.Remove(l_EndLabel);

        // Emit a branch to the end label
        p_IlGenerator.MarkLabel(l_EndLabel);
    }

    private void CompileLoop(ILGenerator p_IlGenerator, WasmLoop p_Instruction)
    {
        // Define the labels for the loop
        Label l_LoopStartLabel = p_IlGenerator.DefineLabel();
        p_IlGenerator.MarkLabel(l_LoopStartLabel);

        // Compile the loop body
        m_CurrentLabels.Add(l_LoopStartLabel);
        EmitInstructions(p_IlGenerator, p_Instruction.Body);
        m_CurrentLabels.Remove(l_LoopStartLabel);
    }

    private void CompileIf(ILGenerator p_IlGenerator, WasmIf p_Instruction)
    {
        // Create the Labels for the if-else block
        Label l_ElseLabel = p_IlGenerator.DefineLabel();
        Label l_EndLabel = p_IlGenerator.DefineLabel();

        p_IlGenerator.Emit(OpCodes.Brfalse, l_ElseLabel);

        // Compile the if block
        EmitInstructions(p_IlGenerator, p_Instruction.IfBody);
        // Emit a branch to the end label
        p_IlGenerator.Emit(OpCodes.Br, l_EndLabel);

        // Compile the else block
        p_IlGenerator.MarkLabel(l_ElseLabel);
        EmitInstructions(p_IlGenerator, p_Instruction.ElseBody);
        // Mark the end label
        p_IlGenerator.MarkLabel(l_EndLabel);
    }

    private void CompileLocalSet(ILGenerator p_IlGenerator, WasmLocalSet p_Instruction)
    {
        if (m_CurrentFuncType.Parameters != null && p_Instruction.LocalIndex < m_CurrentFuncType.Parameters.Length)
        {
            int l_Index = (int)p_Instruction.LocalIndex + 1;
            // Set the parameter value
            p_IlGenerator.Emit(OpCodes.Starg, l_Index);
            
        }
        else
        {
            // Store the local variable
            long l_LocalIndex = p_Instruction.LocalIndex;

            if (m_CurrentFuncType.Parameters != null)
            {
                l_LocalIndex = l_LocalIndex - m_CurrentFuncType.Parameters.Length;
            }

            if (l_LocalIndex == 0)
            {
                p_IlGenerator.Emit(OpCodes.Stloc_0);
            }
            else if (l_LocalIndex == 1)
            {
                p_IlGenerator.Emit(OpCodes.Stloc_1);
            }
            else if (l_LocalIndex == 2)
            {
                p_IlGenerator.Emit(OpCodes.Stloc_2);
            }
            else if (l_LocalIndex == 3)
            {
                p_IlGenerator.Emit(OpCodes.Stloc_3);
            }
            else
            {
                p_IlGenerator.Emit(OpCodes.Stloc, m_CurrentLocals[l_LocalIndex]);   
            }
        }
    }

    private void CompileLocalGet(ILGenerator p_IlGenerator, WasmLocalGet p_Instruction)
    {
        if (m_CurrentFuncType.Parameters != null && p_Instruction.LocalIndex < m_CurrentFuncType.Parameters.Length)
        {
            // Load the parameter 0 is the this parameter

            if (p_Instruction.LocalIndex == 0)
            {
                // Load the first parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_1);
                return;
            }

            if (p_Instruction.LocalIndex == 1)
            {
                // Load the second parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_2);
                return;
            }

            if (p_Instruction.LocalIndex == 2)
            {
                // Load the third parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_3);
                return;
            }

            // Load the parameter
            p_IlGenerator.Emit(OpCodes.Ldarg, p_Instruction.LocalIndex + 1);
        }
        else if (m_CurrentLocals != null)
        {
            long l_LocalIndex = p_Instruction.LocalIndex;

            if (m_CurrentFuncType.Parameters != null)
            {
                l_LocalIndex = l_LocalIndex - m_CurrentFuncType.Parameters.Length;
            }

            if (l_LocalIndex < 0 || l_LocalIndex >= m_CurrentLocals.Length)
                throw new Exception($"Invalid local index: {p_Instruction.LocalIndex}");

            if (l_LocalIndex == 0)
            {
                // Load the first local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_0);
                return;
            }

            if (l_LocalIndex == 1)
            {
                // Load the second local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_1);
                return;
            }

            if (l_LocalIndex == 2)
            {
                // Load the third local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_2);
                return;
            }

            if (l_LocalIndex == 3)
            {
                // Load the fourth local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_3);
                return;
            }

            // Load the local variable
            p_IlGenerator.Emit(OpCodes.Ldloc, l_LocalIndex);
        }
        else
        {
            throw new Exception($"Invalid local index: {p_Instruction.LocalIndex}");
        }
    }
}