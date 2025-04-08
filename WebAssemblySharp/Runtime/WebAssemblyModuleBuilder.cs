using System;

namespace WebAssemblySharp.Runtime;

public class WebAssemblyModuleBuilder
{
    private readonly IWebAssemblyExecutor m_Executor;

    public WebAssemblyModuleBuilder(IWebAssemblyExecutor p_Executor)
    {
        m_Executor = p_Executor;
    }

    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        return m_Executor.GetMethod(p_Name);
    }

    public void DefineImport(string p_Name, Delegate p_Delegate)
    {
        m_Executor.DefineImport(p_Name, p_Delegate);
    }

    public WebAssemblyModule Build()
    {
        m_Executor.OptimizeCode();
        m_Executor.Init();
        return new  WebAssemblyModule(m_Executor);
    }
    
}