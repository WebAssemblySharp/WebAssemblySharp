using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITExecutorVoidMethod: IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Delegate m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, Delegate p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate;
    }

    public async Task<object> Invoke(params object[] p_Args)
    {
        Task l_Task = (Task)m_Delegate.DynamicInvoke(p_Args);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}