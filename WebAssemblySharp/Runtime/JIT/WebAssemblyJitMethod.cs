using System;
using System.Buffers;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitMethod : IWebAssemblyMethod
{
    private readonly IWebAssemblyJitVirtualMaschine m_VirtualMachine;
    private readonly WasmFuncType m_FuncType;
    private readonly WasmCode m_Code;

    public WebAssemblyJitMethod(IWebAssemblyJitVirtualMaschine p_VirtualMachine, WasmFuncType p_FuncType,
        WasmCode p_Code)
    {
        m_FuncType = p_FuncType;
        m_Code = p_Code;
        m_VirtualMachine = p_VirtualMachine;
    }

    public async Task<object> Invoke(params object[] p_Args)
    {
        ValidateParameters(p_Args);

        WebAssemblyJitExecutionContext l_Context = m_VirtualMachine.CreateContext(m_FuncType, m_Code, p_Args);
        bool l_ExecuteFrame = await m_VirtualMachine.ExecuteFrame(l_Context, 0);

        if (!l_ExecuteFrame)
            throw new WebAssemblyJitException("Unexpected abort of the frame execution");

        Span<WebAssemblyJitValue> l_JitValues = m_VirtualMachine.FinishContext(l_Context);

        if (l_JitValues.Length == 0)
            return null;

        if (l_JitValues.Length == 1)
            return l_JitValues[0].Value;
        
        object[] l_Results = new object[l_JitValues.Length];
        
        for (int i = 0; i < l_JitValues.Length; i++)
        {
            l_Results[i] = l_JitValues[i].Value;
        }

        return l_Results;
    }

    

    private void ValidateParameters(object[] p_Args)
    {
        if (p_Args.Length != m_FuncType.Parameters.Length)
            throw new InvalidOperationException("Invalid number of arguments");

        for (int i = 0; i < p_Args.Length; i++)
        {
            Type l_Type = WebAssemblyDataTypeUtils.GetInternalType(m_FuncType.Parameters[i]);

            if (p_Args[i].GetType() != l_Type)
                throw new InvalidOperationException("Invalid argument type at index " + i + " expected " +
                                                    m_FuncType.Parameters[i] + " but got " + p_Args[i].GetType());
        }
    }
}