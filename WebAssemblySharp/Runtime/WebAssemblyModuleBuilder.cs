using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

public class WebAssemblyModuleBuilder
{
    private readonly string m_Name;
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
    private Type m_RuntimeType;
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    private Type m_WrapperInterfaceType;
    private readonly WasmMetaData m_MetaData;
    private Action<WebAssemblyModuleBuilder> m_ExternalConfig;
    private IWebAssemblyExecutor m_Executor;

    public WebAssemblyModuleBuilder(string p_Name, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type p_RuntimeType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type p_WrapperInterfaceType, WasmMetaData p_WasmMetaData, Action<WebAssemblyModuleBuilder> p_Configure)
    {
        m_Name = p_Name;
        m_RuntimeType = p_RuntimeType;
        m_WrapperInterfaceType = p_WrapperInterfaceType;
        m_MetaData = p_WasmMetaData;
        m_ExternalConfig = p_Configure;
    }

    public WasmMetaData MetaData
    {
        get { return m_MetaData; }
    }

    public WebAssemblyModuleBuilder RuntimeType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type p_RuntimeType)
    {
        if (m_Executor != null)
        {
            throw new Exception($"Executor already created, cannot change runtime type");
        }

        m_RuntimeType = p_RuntimeType;
        return this;
    }

    public WebAssemblyModuleBuilder WrapperInterfaceType([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type p_WrapperInterfaceType)
    {
        if (m_Executor != null)
        {
            throw new Exception($"Executor already created, wrapper interface type cannot be changed");
        }    
        
        m_WrapperInterfaceType = p_WrapperInterfaceType;
        return this;
    }

    public WebAssemblyModuleBuilder RuntimeType<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>() where T : IWebAssemblyExecutor
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
            
            if (m_WrapperInterfaceType != null && m_Executor is IWebAssemblyExecutorProxy)
            {
                ((IWebAssemblyExecutorProxy)m_Executor).SetProxyType(m_WrapperInterfaceType);
            }
            
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

    public IWebAssemblyMemoryArea GetMemoryArea(String p_ExportName)
    {
        PreInit();
        return m_Executor.GetMemoryArea(p_ExportName);

    }

    public string Name
    {
        get { return m_Name; }
    }
}