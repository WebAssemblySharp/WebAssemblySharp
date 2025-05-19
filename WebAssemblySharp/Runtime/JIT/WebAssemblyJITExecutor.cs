using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Interpreter;

namespace WebAssemblySharp.Runtime.JIT;

// Starting Point for the JIT/ILCompiler
public class WebAssemblyJITExecutor: IWebAssemblyExecutor, IWebAssemblyExecutorProxy
{
    private WasmMetaData m_WasmMetaData;
    private WebAssemblyJITCompiler m_Compiler;
    private WebAssemblyJITAssembly m_Assembly;
    private Type m_ProxyType;
    
    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        if (m_Assembly == null)
            throw new InvalidOperationException("Assembly not initialized. Call Init() before using GetMethod.");
        
        return (IWebAssemblyMethod)m_Assembly.ExportedMethodes[p_Name];
 
    }

    public void LoadCode(WasmMetaData p_WasmMetaData)
    {
        m_WasmMetaData = p_WasmMetaData;
    }

    public void OptimizeCode()
    {
        m_Compiler = new WebAssemblyJITCompiler(m_WasmMetaData, m_ProxyType);
        m_Compiler.Compile();
    
    }

    public IWebAssemblyMemoryArea GetMemoryArea(string p_Name)
    {
        throw new NotImplementedException();
    }

    public void ImportMemoryArea(string p_Name, IWebAssemblyMemoryArea p_Memory)
    {
        throw new NotImplementedException();
    }

    public void ImportMethod(string p_Name, Delegate p_Delegate)
    {
        throw new NotImplementedException();
    }

    public IWebAssemblyMemoryAreaReadAccess GetInternalMemoryArea(int p_Index = 0)
    {
        throw new NotImplementedException();
    }

    public void SetProxyType(Type p_ProxyType)
    {
        if (!p_ProxyType.IsInterface)
            throw new ArgumentException("Proxy type must be an interface type.", nameof(p_ProxyType));
        
        m_ProxyType = p_ProxyType;
    }

    public T AsInterface<T>()
    {
        if (m_ProxyType == null)
            return default;

        if (!m_ProxyType.IsAssignableFrom(typeof(T)))
            return default;
                
        if (m_Assembly == null)
            throw new InvalidOperationException("Assembly not initialized. Call Init() before using AsInterface.");

        return (T)m_Assembly.Instance;
    }

    public Task Init()
    {
        m_Assembly = m_Compiler.BuildAssembly();
        m_Compiler = null;
        return Task.CompletedTask;
    }
}