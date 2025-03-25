using System;
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

    public object Invoke(params object[] p_Args)
    {
        ValidateParameters(p_Args);

        WebAssemblyJitStackFrame l_Frame = CreateStackFrame(p_Args);

        WebAssemblyJitValue[] l_JitValues = m_VirtualMachine.ExecuteFrame(m_FuncType, m_Code, l_Frame);

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

    private WebAssemblyJitStackFrame CreateStackFrame(object[] p_Args)
    {
        WebAssemblyJitValue[] l_Values = new WebAssemblyJitValue[m_FuncType.Parameters.Length + m_Code.Locals.Length];

        for (int i = 0; i < p_Args.Length; i++)
        {
            l_Values[i] = new WebAssemblyJitValue(m_FuncType.Parameters[i], p_Args[i]);
        }

        for (int i = 0; i < m_Code.Locals.Length; i++)
        {
            WasmCodeLocal l_WasmCodeLocal = m_Code.Locals[i];

            l_Values[(int)l_WasmCodeLocal.Number] = new WebAssemblyJitValue(l_WasmCodeLocal.ValueType, null);
        }

        return new WebAssemblyJitStackFrame(l_Values);
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