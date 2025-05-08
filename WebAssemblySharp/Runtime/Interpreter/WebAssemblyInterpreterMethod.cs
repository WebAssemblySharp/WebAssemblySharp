using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.Interpreter;

public class WebAssemblyInterpreterMethod : IWebAssemblyMethod
{
    private readonly WebAssemblyInterpreterVirtualMaschine m_VirtualMachine;
    private readonly WasmFuncType m_FuncType;
    private readonly WasmCode m_Code;

    public WebAssemblyInterpreterMethod(WebAssemblyInterpreterVirtualMaschine p_VirtualMachine, WasmFuncType p_FuncType,
        WasmCode p_Code)
    {
        m_FuncType = p_FuncType;
        m_Code = p_Code;
        m_VirtualMachine = p_VirtualMachine;
    }

    public async Task<object> Invoke(params object[] p_Args)
    {
        ValidateParameters(p_Args);

        WebAssemblyInterpreterExecutionContext l_Context = m_VirtualMachine.CreateContext(m_FuncType, m_Code, p_Args);
        bool l_ExecuteFrame = await m_VirtualMachine.ExecuteFrame(l_Context, 0);

        if (!l_ExecuteFrame)
            throw new WebAssemblyInterpreterException("Unexpected abort of the frame execution");

        Span<WebAssemblyInterpreterValue> l_JitValues = m_VirtualMachine.FinishContext(l_Context);

        if (l_JitValues.Length == 0)
            return null;

        if (l_JitValues.Length == 1)
            return l_JitValues[0].GetRawValue();
        
        object[] l_Results = new object[l_JitValues.Length];

        // Reverse the order of the results because the stack is LIFO
        for (int i = l_JitValues.Length - 1; i >= 0; i--)
        {
            l_Results[l_JitValues.Length - 1 - i] = l_JitValues[i].GetRawValue();
        }
        
        return l_Results;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncType;
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