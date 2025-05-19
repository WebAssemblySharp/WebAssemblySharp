using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.MetaData.Utils;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITCompiler
{
    private readonly WasmMetaData m_WasmMetaData;
    private readonly Type m_ProxyType;
    private Dictionary<String, MethodInfo> m_ExportedMethods;
    private Dictionary<WasmCode, MethodInfo> m_DynamicMethodsByCode;
    private TypeBuilder m_TypeBuilder;

    private WasmFuncType m_CurrentFuncType;
    private LocalBuilder[] m_CurrentLocals;
    private List<Label> m_CurrentLabels;
    private Label m_ReturnLabel;

    public WebAssemblyJITCompiler(WasmMetaData p_WasmMetaData, Type p_ProxyType)
    {
        m_WasmMetaData = p_WasmMetaData;
        m_ProxyType = p_ProxyType;
    }

    public WebAssemblyJITAssembly BuildAssembly()
    {
        Type l_Type = m_TypeBuilder.CreateType();
        object l_Instance = Activator.CreateInstance(l_Type);
        
        IDictionary l_ExportedMethods = new HybridDictionary();
        
        foreach (KeyValuePair<string, MethodInfo> l_Pair in m_ExportedMethods)
        {
            // Find the function index in the module
            int? l_FunctionIndex = WasmMetaDataUtils.FindExportIndex(m_WasmMetaData, l_Pair.Key, WasmExternalKind.Function);

            if (!l_FunctionIndex.HasValue)
                throw new Exception($"MetaData for Export not found: {l_Pair.Key}");

            long l_FinalIndex = m_WasmMetaData.FuncIndex[l_FunctionIndex.Value];
            WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_FinalIndex];

            MethodInfo l_MethodInfo = l_Type.GetMethod(l_Pair.Key);

            IWebAssemblyMethod l_WebAssemblyMethod = CreateMethod(l_Instance, l_MethodInfo, l_FuncType);
            l_ExportedMethods.Add(l_Pair.Key, l_WebAssemblyMethod);
        }

        m_TypeBuilder = null;
        m_DynamicMethodsByCode = null;
        m_ExportedMethods = null;

        return new WebAssemblyJITAssembly(l_ExportedMethods, l_Instance);
    }

    private IWebAssemblyMethod CreateMethod(object p_Instance, MethodInfo p_MethodInfo, WasmFuncType p_FuncType)
    {
        List<Type> l_ParameterTypes = p_FuncType.Parameters.Select(x => WebAssemblyDataTypeUtils.GetInternalType(x)).ToList();

        if (p_FuncType.Results.Length == 0)
        {
            return CreateVoidMethod(l_ParameterTypes, p_Instance, p_MethodInfo, p_FuncType);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ParameterTypes.Add(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]));
        }
        else
        {
            l_ParameterTypes.Add(typeof(ValueTask<object[]>));
        }

        Type l_DelegateType;

        if (l_ParameterTypes.Count == 0)
        {
            throw new Exception("No Gerneric parameters found");
        }
        else if (l_ParameterTypes.Count == 1)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<>).MakeGenericType(l_ParameterTypes[0]);
        }
        else if (l_ParameterTypes.Count == 2)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1]);
        }
        else if (l_ParameterTypes.Count == 3)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2]);
        }
        else if (l_ParameterTypes.Count == 4)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3]);
        }
        else if (l_ParameterTypes.Count == 5)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4]);
        }
        else if (l_ParameterTypes.Count == 6)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4], l_ParameterTypes[5]);
        }
        else if (l_ParameterTypes.Count == 7)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6]);
        }
        else if (l_ParameterTypes.Count == 8)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7]);
        }
        else if (l_ParameterTypes.Count == 9)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8]);
        }
        else if (l_ParameterTypes.Count == 10)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9]);
        }
        else if (l_ParameterTypes.Count == 11)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10]);
        }
        else if (l_ParameterTypes.Count == 12)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11]);
        }
        else if (l_ParameterTypes.Count == 13)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12]);
        }
        else if (l_ParameterTypes.Count == 14)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13]);
        }
        else if (l_ParameterTypes.Count == 15)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14]);
        }
        else if (l_ParameterTypes.Count == 16)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14],
                l_ParameterTypes[15]);
        }
        else if (l_ParameterTypes.Count == 17)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14],
                l_ParameterTypes[15], l_ParameterTypes[16]);
        }
        else
        {
            throw new Exception($"Too many parameters: {l_ParameterTypes.Count}");
        }

        return (IWebAssemblyMethod)Activator.CreateInstance(l_DelegateType, p_Instance, p_FuncType, p_MethodInfo);
    }

    private IWebAssemblyMethod CreateVoidMethod(List<Type> p_ParameterTypes, object p_Instance, MethodInfo p_MethodInfo, WasmFuncType p_FuncType)
    {
        
        Type l_DelegateType;

        if (p_ParameterTypes.Count == 0)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod);
        }
        else if (p_ParameterTypes.Count == 1)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<>).MakeGenericType(p_ParameterTypes[0]);
        }
        else if (p_ParameterTypes.Count == 2)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1]);
        }
        else if (p_ParameterTypes.Count == 3)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2]);
        }
        else if (p_ParameterTypes.Count == 4)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3]);
        }
        else if (p_ParameterTypes.Count == 5)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4]);
        }
        else if (p_ParameterTypes.Count == 6)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4], p_ParameterTypes[5]);
        }
        else if (p_ParameterTypes.Count == 7)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6]);
        }
        else if (p_ParameterTypes.Count == 8)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7]);
        }
        else if (p_ParameterTypes.Count == 9)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8]);
        }
        else if (p_ParameterTypes.Count == 10)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9]);
        }
        else if (p_ParameterTypes.Count == 11)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10]);
        }
        else if (p_ParameterTypes.Count == 12)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11]);
        }
        else if (p_ParameterTypes.Count == 13)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12]);
        }
        else if (p_ParameterTypes.Count == 14)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12], p_ParameterTypes[13]);
        }
        else if (p_ParameterTypes.Count == 15)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12], p_ParameterTypes[13], p_ParameterTypes[14]);
        }
        else if (p_ParameterTypes.Count == 16)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12], p_ParameterTypes[13], p_ParameterTypes[14],
                p_ParameterTypes[15]);
        }
        else
        {
            throw new Exception($"Too many parameters: {p_ParameterTypes.Count}");
        }

        return (IWebAssemblyMethod)Activator.CreateInstance(l_DelegateType, p_Instance, p_FuncType, p_MethodInfo);
    }

    public void Compile()
    {
        AssemblyName l_AssemblyName = new AssemblyName("DynamicAssembly");
        AssemblyBuilder l_AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(l_AssemblyName, AssemblyBuilderAccess.RunAndCollect);
        ModuleBuilder l_ModuleBuilder = l_AssemblyBuilder.DefineDynamicModule("DynamicModule");

        m_TypeBuilder = l_ModuleBuilder.DefineType("DynamicType", TypeAttributes.Public | TypeAttributes.Sealed);

        if (m_ProxyType != null)
        {
            m_TypeBuilder.AddInterfaceImplementation(m_ProxyType);
        }

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
            l_ReturnType = typeof(ValueTask<object[]>);
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
            MethodInfo l_FromResultMethod = typeof(ValueTask).GetMethod(nameof(ValueTask.FromResult))
                .MakeGenericMethod(typeof(object[]));

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
                break;
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
                break;
            case WasmOpcode.GlobalSet:
                break;
            case WasmOpcode.I32Load:
                break;
            case WasmOpcode.I64Load:
                break;
            case WasmOpcode.F32Load:
                break;
            case WasmOpcode.F64Load:
                break;
            case WasmOpcode.I32Load8S:
                break;
            case WasmOpcode.I32Load8U:
                break;
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
                break;
            case WasmOpcode.I32Store16:
                break;
            case WasmOpcode.I64Store8:
                break;
            case WasmOpcode.I64Store16:
                break;
            case WasmOpcode.I64Store32:
                break;
            case WasmOpcode.MemorySize:
                break;
            case WasmOpcode.MemoryGrow:
                break;
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
                break;
            case WasmOpcode.I32Eq:
                p_IlGenerator.Emit(OpCodes.Ceq);
                return;
            case WasmOpcode.I32Ne:
                break;
            case WasmOpcode.I32LtS:
                break;
            case WasmOpcode.I32LtU:
                p_IlGenerator.Emit(OpCodes.Clt_Un);
                return;
            case WasmOpcode.I32GtS:
                break;
            case WasmOpcode.I32GtU:
                break;
            case WasmOpcode.I32LeS:
                break;
            case WasmOpcode.I32LeU:
                break;
            case WasmOpcode.I32GeS:
                break;
            case WasmOpcode.I32GeU:
                // The following sequence is equivalent to:
                // (a >= b) == !(b < a)
                p_IlGenerator.Emit(OpCodes.Clt_Un); // Lower than
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
                break;
            case WasmOpcode.I32Mul:
                break;
            case WasmOpcode.I32DivS:
                break;
            case WasmOpcode.I32DivU:
                break;
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
                break;
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
            throw new Exception("Cannot set parameter");
        }
        else
        {
            long l_LocalIndex = p_Instruction.LocalIndex;

            if (m_CurrentFuncType.Parameters != null)
            {
                l_LocalIndex = l_LocalIndex - m_CurrentFuncType.Parameters.Length;
            }

            p_IlGenerator.Emit(OpCodes.Stloc, m_CurrentLocals[l_LocalIndex]);
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