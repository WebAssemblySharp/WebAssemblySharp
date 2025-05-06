using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

public class WebAssemblyModuleBuilder
{
    private readonly string m_Name;
    private Type m_RuntimeType;
    private readonly WasmMetaData m_MetaData;
    private Action<WebAssemblyModuleBuilder> m_ExternalConfig;
    private IWebAssemblyExecutor m_Executor;

    public WebAssemblyModuleBuilder(string p_Name, Type p_RuntimeType, WasmMetaData p_WasmMetaData, Action<WebAssemblyModuleBuilder> p_Configure)
    {
        m_Name = p_Name;
        m_RuntimeType = p_RuntimeType;
        m_MetaData = p_WasmMetaData;
        m_ExternalConfig = p_Configure;
    }

    public WasmMetaData MetaData
    {
        get { return m_MetaData; }
    }

    public WebAssemblyModuleBuilder RuntimeType(Type p_RuntimeType)
    {
        if (m_Executor != null)
        {
            throw new Exception($"Executor already created, cannot change runtime type");
        }

        m_RuntimeType = p_RuntimeType;
        return this;
    }

    public WebAssemblyModuleBuilder RuntimeType<T>() where T : IWebAssemblyExecutor
    {
        return RuntimeType(typeof(T));
    }

    public void PreInit()
    {
        if (m_Executor != null)
        {
            // Already initialized
            return;
        }
        
        if (m_Executor == null)
        {
            m_Executor = (IWebAssemblyExecutor)Activator.CreateInstance(m_RuntimeType);
            m_Executor.LoadCode(m_MetaData);
        }
        
        if (m_ExternalConfig != null)
        {
            m_ExternalConfig(this);
            m_ExternalConfig = null;
        }
    }

    public void ImportMethod(string p_Name, Delegate p_Delegate)
    {
        PreInit();
        m_Executor.ImportMethod(p_Name, p_Delegate);
    }
    
    public void ImportMemoryArea(string p_Name, IWebAssemblyMemoryArea p_Memory)
    {
        PreInit();
        m_Executor.ImportMemoryArea(p_Name, p_Memory);
    }

    public async Task<WebAssemblyModule> Build()
    {
        PreInit();

        m_Executor.OptimizeCode();
        await m_Executor.Init();
        return new WebAssemblyModule(m_Name, m_Executor);
    }

    public IWebAssemblyMethod GetMethod(String p_ExportName)
    {
        PreInit();
        return m_Executor.GetMethod(p_ExportName);
    }

    public IWebAssemblyMemoryArea GetMemoryArea(WasmString p_ExportName)
    {
        PreInit();

        WasmExport l_WasmExport = m_MetaData.Export.FirstOrDefault(x => x.Name == p_ExportName && x.Kind == WasmExternalKind.Memory);

        if (l_WasmExport == null)
            return null;

        return m_Executor.GetMemoryArea((int)l_WasmExport.Index);

    }
}