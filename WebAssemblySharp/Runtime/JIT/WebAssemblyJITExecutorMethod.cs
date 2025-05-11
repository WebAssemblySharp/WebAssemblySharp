using System;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITExecutorMethod<T>: IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Delegate m_Delegate;
    
    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, Delegate p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate;
    }

    public async Task<object> Invoke(params object[] p_Args)
    {
        T l_Result = await (Task<T>)m_Delegate.DynamicInvoke(p_Args);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}