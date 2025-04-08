using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

/*
 * Represents an instantiated WebAssembly module.
 *
 * This class serves as a wrapper around the underlying executor implementation,
 * providing a simplified interface for invoking WebAssembly functions. It acts as
 * the main entry point for client code to interact with compiled/loaded WebAssembly modules.
 *
 * The module maintains a reference to its executor, which is responsible for the actual
 * execution strategy (interpretation or JIT compilation) and provides access to the
 * module's functions.
 *
 * Key responsibilities:
 * - Providing access to WebAssembly functions by name
 * - Handling function invocation with appropriate parameters
 * - Abstracting the execution details from the client code
 */
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

    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        return m_Executor.GetMethod(p_Name);
    }
    
}