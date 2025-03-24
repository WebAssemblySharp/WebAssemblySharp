using System;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitMethod : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncType;
    private readonly WasmCode m_Code;

    public WebAssemblyJitMethod(WasmFuncType p_FuncType, WasmCode p_Code)
    {
        m_FuncType = p_FuncType;
        m_Code = p_Code;
    }

    public object Invoke(params object[] p_Args)
    {
        ValidateParameters(p_Args);

        return null;
    }

    private void ValidateParameters(object[] p_Args)
    {
        if (p_Args.Length != m_FuncType.Parameters.Length)
            throw new InvalidOperationException("Invalid number of arguments");
        
        for (int i = 0; i < p_Args.Length; i++)
        {
            Type l_Type = WebAssemblyDataTypeUtils.GetInternalType(m_FuncType.Parameters[i]);
            
            if (p_Args[i].GetType() != l_Type)
                throw new InvalidOperationException("Invalid argument type at index " + i + " expected " + m_FuncType.Parameters[i] + " but got " + p_Args[i].GetType());
        }
    }
}