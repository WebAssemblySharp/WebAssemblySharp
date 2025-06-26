using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Utils;
using WebAssemblySharp.Runtime.Interpreter;

namespace WebAssemblySharp.Runtime.JIT;

// Starting Point for the JIT/ILCompiler
public class WebAssemblyJITExecutor: IWebAssemblyExecutor, IWebAssemblyExecutorProxy
{
    private WasmMetaData m_WasmMetaData;
    private WebAssemblyJITRuntimeCompiler m_RuntimeCompiler;
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
        m_RuntimeCompiler = new WebAssemblyJITRuntimeCompiler(m_WasmMetaData, m_ProxyType);
        m_RuntimeCompiler.Compile();
    
    }

    public IWebAssemblyMemoryArea GetMemoryArea(string p_Name)
    {
        int? l_ExportIndex = WasmMetaDataUtils.FindExportIndex(m_WasmMetaData, p_Name, WasmExternalKind.Memory);

        if (!l_ExportIndex.HasValue)
        {
            throw new Exception("Export not found: " + p_Name);
        }
        
        return m_Assembly.MemoryAreas[l_ExportIndex.Value];
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
        return m_Assembly.MemoryAreas[p_Index];
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
        m_Assembly = m_RuntimeCompiler.BuildAssembly();
        m_RuntimeCompiler = null;
        return Task.CompletedTask;
    }
}