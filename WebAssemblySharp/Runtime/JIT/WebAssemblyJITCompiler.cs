using System;
using System.Collections.Generic;
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
    private Dictionary<String, MethodInfo> m_ExportedMethods;
    private Dictionary<WasmCode, DynamicMethod> m_DynamicMethodsByCode;

    private WasmFuncType m_CurrentFuncType;
    private LocalBuilder[] m_CurrentLocals;

    public WebAssemblyJITCompiler(WasmMetaData p_WasmMetaData)
    {
        m_WasmMetaData = p_WasmMetaData;
    }

    public WebAssemblyJITAssembly BuildAssembly()
    {
        Dictionary<string, IWebAssemblyMethod> l_ExportedMethods = new Dictionary<string, IWebAssemblyMethod>();

        foreach (KeyValuePair<string, MethodInfo> l_Pair in m_ExportedMethods)
        {
            // Find the function index in the module
            int? l_FunctionIndex = WasmMetaDataUtils.FindExportIndex(m_WasmMetaData, l_Pair.Key, WasmExternalKind.Function);

            if (!l_FunctionIndex.HasValue)
                throw new Exception($"MetaData for Export not found: {l_Pair.Key}");

            long l_FinalIndex = m_WasmMetaData.FuncIndex[l_FunctionIndex.Value];
            WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_FinalIndex];

            Delegate l_Delegate = CreateDelegate(l_Pair.Value, l_FuncType);

            if (l_FuncType.Results.Length == 0)
            {
                l_ExportedMethods.Add(l_Pair.Key, new WebAssemblyJITExecutorVoidMethod(l_FuncType, l_Delegate));
            }
            else if (l_FuncType.Results.Length == 1)
            {
                Type l_MethodTyp = typeof(WebAssemblyJITExecutorMethod<>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(l_FuncType.Parameters[0]));
                l_ExportedMethods.Add(l_Pair.Key, (IWebAssemblyMethod)Activator.CreateInstance(l_MethodTyp, l_FuncType, l_Delegate));    
            }
            else
            {
                l_ExportedMethods.Add(l_Pair.Key, new WebAssemblyJITExecutorMethod<object[]>(l_FuncType, l_Delegate));
            }
            
            
        }


        return new WebAssemblyJITAssembly(l_ExportedMethods);
    }

    private Delegate CreateDelegate(MethodInfo p_Method, WasmFuncType p_FuncType)
    {
        List<Type> l_ParameterTypes = p_FuncType.Parameters.Select(x => WebAssemblyDataTypeUtils.GetInternalType(x)).ToList();

        if (p_FuncType.Results.Length == 0)
        {
            l_ParameterTypes.Add(typeof(Task));
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ParameterTypes.Add(typeof(Task<>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0])));
        }
        else
        {
            l_ParameterTypes.Add(typeof(Task<object[]>));
        }

        Type l_DelegateType;
        
        if (l_ParameterTypes.Count == 0)
        {
            throw new InvalidOperationException("No parameter types found");
        }
        else if (l_ParameterTypes.Count == 1)
        {
            l_DelegateType = typeof(Func<>).MakeGenericType(l_ParameterTypes[0]);
        }
        else if (l_ParameterTypes.Count == 2)
        {
            l_DelegateType = typeof(Func<,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1]);
        }
        else if (l_ParameterTypes.Count == 3)
        {
            l_DelegateType = typeof(Func<,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2]);
        }
        else if (l_ParameterTypes.Count == 4)
        {
            l_DelegateType = typeof(Func<,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3]);
        }
        else if (l_ParameterTypes.Count == 5)
        {
            l_DelegateType = typeof(Func<,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4]);
        }
        else if (l_ParameterTypes.Count == 6)
        {
            l_DelegateType = typeof(Func<,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5]);
        }
        else if (l_ParameterTypes.Count == 7)
        {
            l_DelegateType = typeof(Func<,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6]);
        }
        else if (l_ParameterTypes.Count == 8)
        {
            l_DelegateType = typeof(Func<,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7]);
        }
        else if (l_ParameterTypes.Count == 9)
        {
            l_DelegateType = typeof(Func<,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8]);
        }
        else if (l_ParameterTypes.Count == 10)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9]);
        }
        else if (l_ParameterTypes.Count == 11)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10]);
        }
        else if (l_ParameterTypes.Count == 12)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11]);
        }
        else if (l_ParameterTypes.Count == 13)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12]);
        }
        else if (l_ParameterTypes.Count == 14)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13]);
        }
        else if (l_ParameterTypes.Count == 15)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14]);
        }
        else if (l_ParameterTypes.Count == 16)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14], l_ParameterTypes[15]);
        }
        else if (l_ParameterTypes.Count == 17)
        {
            l_DelegateType = typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8], l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14], l_ParameterTypes[15], l_ParameterTypes[16]);
        }
        else 
        {
            throw new Exception($"Too many parameters: {l_ParameterTypes.Count}");
        }
        
        return p_Method.CreateDelegate(l_DelegateType);
    }

    public void Compile()
    {
        m_ExportedMethods = new Dictionary<string, MethodInfo>();
        m_DynamicMethodsByCode = new Dictionary<WasmCode, DynamicMethod>();

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
            MethodInfo l_Method = CompileCode(l_Export.Name, l_FuncType, l_Code);
            m_ExportedMethods.Add(l_Export.Name, l_Method);
        }
    }

    private MethodInfo CompileCode(string p_ExportName, WasmFuncType p_FuncType, WasmCode p_Code)
    {
        if (m_ExportedMethods.TryGetValue(p_ExportName, out var l_Method))
            return l_Method;

        if (m_DynamicMethodsByCode.TryGetValue(p_Code, out var l_DynamicMethod))
            return l_DynamicMethod;

        Type l_ReturnType;

        if (p_FuncType.Results.Length == 0)
        {
            l_ReturnType = typeof(Task);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ReturnType = typeof(Task<>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]));
        }
        else
        {
            l_ReturnType = typeof(Task<object[]>);
        }

        DynamicMethod l_NewMethod = new DynamicMethod(p_ExportName, l_ReturnType,
            p_FuncType.Parameters.Select(x => WebAssemblyDataTypeUtils.GetInternalType(x)).ToArray(),
            typeof(WebAssemblyJITCompiler).Module);

        m_DynamicMethodsByCode.Add(p_Code, l_NewMethod);

        ILGenerator l_IlGenerator = l_NewMethod.GetILGenerator();

        
        m_CurrentFuncType = p_FuncType;
        m_CurrentLocals = new LocalBuilder[p_Code.Locals != null ? p_Code.Locals.Length + 2 : 2];
        
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
        
        LocalBuilder l_AsyncMethodBuilderLocalBuilder;
        
        if (p_FuncType.Results.Length == 0)
        {
            l_AsyncMethodBuilderLocalBuilder = l_IlGenerator.DeclareLocal(typeof(AsyncTaskMethodBuilder));
        }
        else if (p_FuncType.Results.Length == 1)
        {
            Type l_InternalType = WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]);
            l_AsyncMethodBuilderLocalBuilder = l_IlGenerator.DeclareLocal(typeof(AsyncTaskMethodBuilder<>).MakeGenericType(l_InternalType));
        }
        else
        {
            l_AsyncMethodBuilderLocalBuilder = l_IlGenerator.DeclareLocal(typeof(AsyncTaskMethodBuilder<object[]>));
        }
        
        l_IlGenerator.Emit(OpCodes.Ldloca_S, l_AsyncMethodBuilderLocalBuilder);
        
        // Emit IL for each instruction in the WebAssembly function
        foreach (WasmInstruction l_Instruction in p_Code.Instructions)
        {
            EmitInstruction(l_IlGenerator, l_Instruction);
        }
        
        l_IlGenerator.Emit(OpCodes.Call, typeof(AsyncTaskMethodBuilder<int>).GetMethod("SetResult"));

        // 5. Task zurückgeben
        l_IlGenerator.Emit(OpCodes.Ldloca_S, l_AsyncMethodBuilderLocalBuilder);
        l_IlGenerator.Emit(OpCodes.Call, typeof(AsyncTaskMethodBuilder<int>).GetMethod("get_Task"));
        l_IlGenerator.Emit(OpCodes.Ret);

        m_CurrentLocals = null;
        m_CurrentFuncType = null;

        return l_NewMethod;
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
                break;
            case WasmOpcode.Loop:
                break;
            case WasmOpcode.If:
                break;
            case WasmOpcode.Else:
                break;
            case WasmOpcode.End:
                break;
            case WasmOpcode.Br:
                break;
            case WasmOpcode.BrIf:
                break;
            case WasmOpcode.BrTable:
                break;
            case WasmOpcode.Return:
                break;
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
                break;
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
                break;
            case WasmOpcode.I64Const:
                break;
            case WasmOpcode.F32Const:
                break;
            case WasmOpcode.F64Const:
                break;
            case WasmOpcode.I32Eqz:
                break;
            case WasmOpcode.I32Eq:
                break;
            case WasmOpcode.I32Ne:
                break;
            case WasmOpcode.I32LtS:
                break;
            case WasmOpcode.I32LtU:
                break;
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
                break;
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
                break;
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

    private void CompileLocalGet(ILGenerator p_IlGenerator, WasmLocalGet p_Instruction)
    {
        if (m_CurrentFuncType.Parameters != null && p_Instruction.LocalIndex < m_CurrentFuncType.Parameters.Length)
        {
            if (p_Instruction.LocalIndex == 0)
            {
                // Load the first parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_0);
                return;
            }
            
            if (p_Instruction.LocalIndex == 1)
            {
                // Load the second parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_1);
                return;
            }
            
            if (p_Instruction.LocalIndex == 2)
            {
                // Load the third parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_2);
                return;
            }
            
            if (p_Instruction.LocalIndex == 3)
            {
                // Load the fourth parameter
                p_IlGenerator.Emit(OpCodes.Ldarg_3);
                return;
            }
            
            // Load the parameter
            p_IlGenerator.Emit(OpCodes.Ldarg, p_Instruction.LocalIndex);
        }
        else if (m_CurrentLocals != null && p_Instruction.LocalIndex < m_CurrentLocals.Length)
        {
            if (p_Instruction.LocalIndex == 0)
            {
                // Load the first local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_0);
                return;
            }
            
            if (p_Instruction.LocalIndex == 1)
            {
                // Load the second local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_1);
                return;
            }
            
            if (p_Instruction.LocalIndex == 2)
            {
                // Load the third local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_2);
                return;
            }
            
            if (p_Instruction.LocalIndex == 3)
            {
                // Load the fourth local variable
                p_IlGenerator.Emit(OpCodes.Ldloc_3);
                return;
            }
            
            // Load the local variable
            p_IlGenerator.Emit(OpCodes.Ldloc, p_Instruction.LocalIndex);
        }
        else
        {
            throw new Exception($"Invalid local index: {p_Instruction.LocalIndex}");
        }
    }
}