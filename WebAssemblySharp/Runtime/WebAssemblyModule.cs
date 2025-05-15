using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Interpreter;

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
    public String Name { get; }
    
    public WebAssemblyModule(string p_Name, IWebAssemblyExecutor p_Executor)
    {
        m_Executor = p_Executor;
        Name = p_Name;
    }

    public ValueTask<object> DynamicCall(string p_Name, params object[] p_Args)
    {
        return m_Executor.GetMethod(p_Name).DynamicInvoke(p_Args);
    }
    
    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        return m_Executor.GetMethod(p_Name);
    }

    public IWebAssemblyMemoryArea GetMemoryArea(string p_Name)
    {
        return m_Executor.GetMemoryArea(p_Name);
    }
    
    public ValueTask CallVoid(string p_Name)
    {
        Func<ValueTask> l_Delegate = m_Executor.GetMethod(p_Name).GetVoidDelegate();
        return l_Delegate();
    }
    
    public ValueTask CallVoid<TInput1>(string p_Name, TInput1 p_Input1) where TInput1: struct
    {
        Func<TInput1, ValueTask> l_Delegate = m_Executor.GetMethod(p_Name).GetVoidDelegate<TInput1>();
        return l_Delegate(p_Input1);
    }
    
    public ValueTask CallVoid<TInput1, TInput2>(string p_Name, TInput1 p_Input1, TInput2 p_Input2) where TInput1: struct where TInput2: struct
    {
        Func<TInput1, TInput2, ValueTask> l_Delegate = m_Executor.GetMethod(p_Name).GetVoidDelegate<TInput1, TInput2>();
        return l_Delegate(p_Input1, p_Input2);
    }
    
    public ValueTask CallVoid<TInput1, TInput2, TInput3>(string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3) where TInput1: struct where TInput2: struct where TInput3: struct
    {
        Func<TInput1, TInput2, TInput3, ValueTask> l_Delegate = m_Executor.GetMethod(p_Name).GetVoidDelegate<TInput1, TInput2, TInput3>();
        return l_Delegate(p_Input1, p_Input2, p_Input3);
    }
    
    public ValueTask CallVoid<TInput1, TInput2, TInput3, TInput4>(string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct
    {
        Func<TInput1, TInput2, TInput3, TInput4, ValueTask> l_Delegate = m_Executor.GetMethod(p_Name).GetVoidDelegate<TInput1, TInput2, TInput3, TInput4>();
        return l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4);
    }
    
    public ValueTask CallVoid<TInput1, TInput2, TInput3, TInput4, TInput5>(string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask> l_Delegate = m_Executor.GetMethod(p_Name).GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5>();
        return l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5);
    }
    
    
    public ValueTask<TResult> Call<TResult>(string p_Name) where TResult: struct
    {
        Func<ValueTask<TResult>> l_Delegate = m_Executor.GetMethod(p_Name).GetDelegate<TResult>();
        return l_Delegate();
    }
    
    public ValueTask<TResult> Call<TResult, TInput1>(string p_Name, TInput1 p_Input1) where TResult: struct where TInput1: struct
    {
        Func<TInput1, ValueTask<TResult>> l_Delegate = m_Executor.GetMethod(p_Name).GetDelegate<TInput1, TResult>();
        return l_Delegate(p_Input1);
    }
    
    public ValueTask<TResult> Call<TResult, TInput1, TInput2>(string p_Name, TInput1 p_Input1, TInput2 p_Input2) where TResult: struct where TInput1: struct where TInput2: struct
    {
        Func<TInput1, TInput2, ValueTask<TResult>> l_Delegate = m_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TResult>();
        return l_Delegate(p_Input1, p_Input2);
    }
    
    public ValueTask<TResult> Call<TResult, TInput1, TInput2, TInput3>(string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3) where TResult: struct where TInput1: struct where TInput2: struct where TInput3: struct
    {
        Func<TInput1, TInput2, TInput3, ValueTask<TResult>> l_Delegate = m_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TResult>();
        return l_Delegate(p_Input1, p_Input2, p_Input3);
    }
    
    public ValueTask<TResult> Call<TResult, TInput1, TInput2, TInput3, TInput4>(string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4) where TResult: struct where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct
    {
        Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>> l_Delegate = m_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TResult>();
        return l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4);
    }
    
    public ValueTask<TResult> Call<TResult, TInput1, TInput2, TInput3, TInput4, TInput5>(string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5) where TResult: struct where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>> l_Delegate = m_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TResult>();
        return l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5);
    }
    
}