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
    private WebAssemblyJITAssembly m_Assembly;
    private WebAssemblyJITUnfinishedAssembly m_UnfinishedAssembly;
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private Type m_ProxyType;
    private IDictionary<int, IWebAssemblyMemoryArea> m_ImportedMemoryAreas;
    private IDictionary<int, Delegate> m_ImportMethods;

    public static bool IsSupported
    {
        get
        {
            return RuntimeFeature.IsDynamicCodeSupported;     
        }
    } 
    
    public WebAssemblyJITExecutor()
    {
        m_ImportedMemoryAreas = new Dictionary<int, IWebAssemblyMemoryArea>();
        m_ImportMethods = new Dictionary<int, Delegate>();
    }

    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        if (m_Assembly != null)
            return (IWebAssemblyMethod)m_Assembly.ExportedMethodes[p_Name];
        
        if (m_UnfinishedAssembly != null)
            return (IWebAssemblyMethod)m_UnfinishedAssembly.ExportedMethodes[p_Name];


        throw new InvalidOperationException("Assembly not initialized. Call Init() before using GetMethod.");
        
    }

    public void LoadCode(WasmMetaData p_WasmMetaData)
    {
        m_WasmMetaData = p_WasmMetaData;
        WebAssemblyJITRuntimeCompiler l_RuntimeCompiler = new WebAssemblyJITRuntimeCompiler(m_WasmMetaData, m_ProxyType);
        l_RuntimeCompiler.Compile();
        m_UnfinishedAssembly = l_RuntimeCompiler.BuildAssembly();
    }

    public void OptimizeCode()
    {
        m_ImportedMemoryAreas = new ReadOnlyDictionary<int, IWebAssemblyMemoryArea>(m_ImportedMemoryAreas);
        m_ImportMethods = new ReadOnlyDictionary<int, Delegate>(m_ImportMethods);
    }

    public IWebAssemblyMemoryArea GetMemoryArea(string p_Name)
    {
        int? l_ExportIndex = WasmMetaDataUtils.FindExportIndex(m_WasmMetaData, p_Name, WasmExternalKind.Memory);

        if (!l_ExportIndex.HasValue)
        {
            throw new Exception("Export not found: " + p_Name);
        }

        if (m_Assembly != null)
        {
            return m_Assembly.MemoryAreas[l_ExportIndex.Value];    
        }
        
        if (m_UnfinishedAssembly != null)
        {
            return m_UnfinishedAssembly.MemoryAreas[l_ExportIndex.Value];
        }
        
        throw new InvalidOperationException("Assembly not initialized. Call Init() before using GetMemoryArea."); 
        
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
        
        if (m_UnfinishedAssembly != null || m_Assembly != null)
            throw new InvalidOperationException("Proxy type cannot be set after assembly has been initialized.");
        
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
        m_Assembly = m_UnfinishedAssembly.Finalize(m_ImportedMemoryAreas, m_ImportMethods);
        m_UnfinishedAssembly = null;
        return Task.CompletedTask;
    }
}