using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

public class WebAssemblyModule
{
    private readonly IWebAssemblyExecutor m_Executor;
    
    public WebAssemblyModule(IWebAssemblyExecutor p_Executor)
    {
        m_Executor = p_Executor;
    }

    public Task<object> Call(string p_Name, params object[] p_Args)
    {
        return m_Executor.GetMethod(p_Name).Invoke(p_Args);
    }
}