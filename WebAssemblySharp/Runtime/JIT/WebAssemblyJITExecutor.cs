using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Utils;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

// Starting Point for the JIT/ILCompiler
[RequiresDynamicCode("WebAssemblyJITExecutor requires dynamic code.")]
public class WebAssemblyJITExecutor: IWebAssemblyExecutor, IWebAssemblyExecutorProxy
{
    private WasmMetaData m_WasmMetaData;
    private WebAssemblyJITRuntimeCompiler m_RuntimeCompiler;
    private WebAssemblyJITAssembly m_Assembly;
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private Type m_ProxyType;
    private IDictionary<int, IWebAssemblyMemoryArea> m_ImportedMemoryAreas;
    private IDictionary<int, Delegate> m_ImportMethods;

    public WebAssemblyJITExecutor()
    {
        m_ImportedMemoryAreas = new Dictionary<int, IWebAssemblyMemoryArea>();
        m_ImportMethods = new Dictionary<int, Delegate>();
    }

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
        m_ImportedMemoryAreas = new ReadOnlyDictionary<int, IWebAssemblyMemoryArea>(m_ImportedMemoryAreas);
        m_ImportMethods = new ReadOnlyDictionary<int, Delegate>(m_ImportMethods);
        
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
        WasmImportMemory l_Import = WebAssemblyImportUtils.FindImportByName<WasmImportMemory>(m_WasmMetaData, p_Name);

        if (l_Import == null)
            throw new Exception("Import not found: " + p_Name);

        if (p_Memory.GetCurrentPages() < l_Import.Min)
            throw new Exception($"Import memory {p_Name} area too small. Expected min {l_Import.Min} pages, got {p_Memory.GetCurrentPages()} pages");

        if (p_Memory.GetMaximumPages() < l_Import.Max)
            throw new Exception($"Import memory {p_Name} area too small. Expected max {l_Import.Max} pages, got {p_Memory.GetMaximumPages()} pages");
        
        m_ImportedMemoryAreas.Add(0, p_Memory);
    }

    public void ImportMethod(string p_Name, Delegate p_Delegate)
    {
        WasmImportFunction l_Import = WebAssemblyImportUtils.FindImportByName<WasmImportFunction>(m_WasmMetaData, p_Name);

        if (l_Import == null)
            throw new Exception("Import not found: " + p_Name);
        
        if (!m_ImportMethods.TryAdd((int)l_Import.FunctionIndex, p_Delegate))
            throw new Exception("Import already defined: " + p_Name);
    }

    public IWebAssemblyMemoryAreaReadAccess GetInternalMemoryArea(int p_Index = 0)
    {
        return m_Assembly.MemoryAreas[p_Index];
    }

    public void SetProxyType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type p_ProxyType)
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
        m_Assembly = m_RuntimeCompiler.BuildAssembly(m_ImportedMemoryAreas, m_ImportMethods);
        m_RuntimeCompiler = null;
        return Task.CompletedTask;
    }
}