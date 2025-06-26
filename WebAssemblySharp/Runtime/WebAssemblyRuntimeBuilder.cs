using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.Attributes;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Readers.Binary;
using WebAssemblySharp.Runtime.Interpreter;

namespace WebAssemblySharp.Runtime;

/*
 * The WebAssemblyRuntimeBuilder provides a fluent interface for constructing WebAssembly runtime environments.
 * 
 * This class is responsible for:
 * - Loading and managing WebAssembly modules from binary streams or metadata
 * - Connecting imports and exports between modules
 * - Validating module dependencies
 * - Building the complete WebAssembly runtime environment
 * 
 * The builder supports both single-module and multi-module scenarios, with helper methods
 * for quickly creating single-module runtimes and more detailed configuration for
 * complex multi-module setups.
 * 
 * Usage example:
 *   var builder = WebAssemblyRuntimeBuilder.Create();
 *   await builder.LoadModule("module1", stream1);
 *   await builder.LoadModule("module2", stream2);
 *   var runtime = await builder.Build();
 */
public class WebAssemblyRuntimeBuilder
{
    private Type m_ExecutorType;
    
    private Dictionary<string, WebAssemblyModuleBuilder> m_Modules;
    
    private Dictionary<string, Delegate> m_ImportedMethods;
    private Dictionary<string, IWebAssemblyMemoryArea> m_ImportedMemoryAreas;
    
    private bool m_ValidateImportAndExport;

    public static WebAssemblyRuntimeBuilder Create(Type p_RuntimeType)
    {
        return new WebAssemblyRuntimeBuilder(p_RuntimeType);
    }
    
    public static WebAssemblyRuntimeBuilder Create<T>() where T : IWebAssemblyExecutor
    {
        return new WebAssemblyRuntimeBuilder(typeof(T));
    }
    
    public static WebAssemblyRuntimeBuilder Create()
    {
        return new WebAssemblyRuntimeBuilder(typeof(WebAssemblyInterpreterExecutor));
    }

    public static async Task<T> CreateSingleModuleRuntime<T>(Action<WebAssemblyModuleBuilder> p_Configure = null)
    
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = Create();
        await l_RuntimeBuilder.LoadModule(typeof(T), p_Configure);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        
        WebAssemblyModuleDefinitionAttribute l_ModuleDefinitionAttribute = typeof(T).GetCustomAttribute<WebAssemblyModuleDefinitionAttribute>();
        return l_WebAssemblyRuntime.GetModule(l_ModuleDefinitionAttribute.Name).AsInterface<T>();
    }

    public static async Task<WebAssemblyModule> CreateSingleModuleRuntime(Stream p_Stream, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = Create();
        await l_RuntimeBuilder.LoadModule("main", p_Stream, null, p_Configure);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        return l_WebAssemblyRuntime.GetModule("main");
    }
    
    public static async Task<WebAssemblyModule> CreateSingleModuleRuntime(Type p_RuntimeType, Stream p_Stream, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = Create(p_RuntimeType);
        await l_RuntimeBuilder.LoadModule("main", p_Stream, null, p_Configure);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        return l_WebAssemblyRuntime.GetModule("main");
    }
    
    public static async Task<WebAssemblyModule> CreateSingleModuleRuntime(Type p_RuntimeType, Type p_InterfaceWrapper, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = Create(p_RuntimeType);
        await l_RuntimeBuilder.LoadModule(p_InterfaceWrapper, p_Configure);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();
        
        WebAssemblyModuleDefinitionAttribute l_ModuleDefinitionAttribute = p_InterfaceWrapper.GetCustomAttribute<WebAssemblyModuleDefinitionAttribute>();
        return l_WebAssemblyRuntime.GetModule(l_ModuleDefinitionAttribute.Name);
    }
    
    public static async Task<T> CreateSingleModuleRuntime<T>(Type p_RuntimeType, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        WebAssemblyRuntimeBuilder l_RuntimeBuilder = Create(p_RuntimeType);
        await l_RuntimeBuilder.LoadModule(typeof(T), p_Configure);
        WebAssemblyRuntime l_WebAssemblyRuntime = await l_RuntimeBuilder.Build();

        WebAssemblyModuleDefinitionAttribute l_ModuleDefinitionAttribute = typeof(T).GetCustomAttribute<WebAssemblyModuleDefinitionAttribute>();
        return l_WebAssemblyRuntime.GetModule(l_ModuleDefinitionAttribute.Name).AsInterface<T>();
    }

    public WebAssemblyRuntimeBuilder(Type p_ExecutorType)
    {
        m_ValidateImportAndExport = true;
        m_ExecutorType = p_ExecutorType;
        m_Modules = new Dictionary<string, WebAssemblyModuleBuilder>();
    }

    public WebAssemblyRuntimeBuilder ValidateImportAndExport(bool p_Validate)
    {
        m_ValidateImportAndExport = p_Validate;
        return this;
    }

    public WebAssemblyModuleBuilder GetModule(String p_Module)
    {
        if (!m_Modules.TryGetValue(p_Module, out WebAssemblyModuleBuilder l_ModuleBuilder))
        {
            throw new Exception($"Cannot find module {p_Module}");
        }
        
        return l_ModuleBuilder;
    }

    public Task<WebAssemblyRuntimeBuilder> LoadModule(Type p_InterfaceType, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        WebAssemblyModuleDefinitionAttribute l_ModuleDefinitionAttribute = p_InterfaceType.GetCustomAttribute<WebAssemblyModuleDefinitionAttribute>();

        if (l_ModuleDefinitionAttribute == null)
        {
            throw new Exception($"Cannot find WebAssemblyModuleDefinitionAttribute on type {p_InterfaceType.FullName}");
        }

        string l_Name = l_ModuleDefinitionAttribute.Name;

        using (Stream l_Stream = LoadStreamByInterfaceType(p_InterfaceType))
        {
            return LoadModule(l_Name, l_Stream, p_InterfaceType, p_Configure);
        }
    }

    private Stream LoadStreamByInterfaceType(Type p_InterfaceType)
    {
        WebAssemblyModuleManifestResource l_ManifestResource = p_InterfaceType.GetCustomAttribute<WebAssemblyModuleManifestResource>();

        Stream l_Stream = null;
        
        if (l_ManifestResource != null)
        {
            Assembly l_Assembly = l_ManifestResource.AssemblyType?.Assembly ?? p_InterfaceType.Assembly;
            l_Stream = l_Assembly.GetManifestResourceStream(l_ManifestResource.Location);
        }
        
        // QQQ Handle outer Types
        
        if (l_Stream != null)
        {
            return l_Stream;
        }
        
        throw new Exception("Unable to find Wasm Steam for type " + p_InterfaceType.FullName);
        
    }

    public Task<WebAssemblyRuntimeBuilder> LoadModule<T>(Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        return LoadModule(typeof(T), p_Configure);
    }
    
    public async Task<WebAssemblyRuntimeBuilder> LoadModule(String p_Module, Stream p_Stream, Type p_WrapperInterfaceType = null, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        WasmBinaryReader l_Reader = new WasmBinaryReader();

        byte[] l_Bytes = new Byte[255];

        while (true)
        {
            int l_ReadBlock = await p_Stream.ReadAsync(l_Bytes);

            if (l_ReadBlock != 0)
            {
                l_Reader.Read(l_Bytes.AsSpan().Slice(0, l_ReadBlock));
            }

            if (l_ReadBlock < l_Bytes.Length)
            {
                break;
            }
        }

        WasmMetaData l_WasmMetaData = l_Reader.Finish();
        return LoadModule(p_Module, p_WrapperInterfaceType, l_WasmMetaData, p_Configure);
    }

    public WebAssemblyRuntimeBuilder LoadModule(string p_Module, Type p_WrapperInterfaceType, WasmMetaData p_WasmMetaData, Action<WebAssemblyModuleBuilder> p_Configure = null)
    {
        if (m_Modules.ContainsKey(p_Module))
        {
            throw new Exception($"Module {p_Module} already loaded");
        }

        WebAssemblyModuleBuilder l_ModuleBuilder = new WebAssemblyModuleBuilder(p_Module, m_ExecutorType, p_WrapperInterfaceType, p_WasmMetaData, p_Configure);
        m_Modules.Add(p_Module, l_ModuleBuilder);
        return this;
    }

    public async Task<WebAssemblyRuntime> Build()
    {
        // Connect Imports and Exports

        foreach (WebAssemblyModuleBuilder l_WebAssemblyModuleBuilder in m_Modules.Values)
        {
            ConnectImports(l_WebAssemblyModuleBuilder);
        }

        // Build all modules
        List<WebAssemblyModule> l_Modules = new List<WebAssemblyModule>(m_Modules.Count);

        foreach (WebAssemblyModuleBuilder l_WebAssemblyModuleBuilder in m_Modules.Values)
        {
            WebAssemblyModule l_ModuleInstance = await l_WebAssemblyModuleBuilder.Build();
            l_Modules.Add(l_ModuleInstance);
        }

        return new WebAssemblyRuntime(l_Modules);
    }

    private void ConnectImports(WebAssemblyModuleBuilder p_ImportModule)
    {
        WasmImport[] l_Import = p_ImportModule.MetaData.Import;

        if (l_Import == null || l_Import.Length == 0)
        {
            return;
        }

        foreach (WasmImport l_WasmImport in l_Import)
        {
            if (l_WasmImport.Module == WebAssemblyConst.WASM_HOST_MODULE_NAME)
            {
                if (l_WasmImport.Kind == WasmExternalKind.Function)
                {
                    if (m_ImportedMethods == null)
                        continue;

                    if (!m_ImportedMethods.TryGetValue(l_WasmImport.Name, out Delegate l_Delegate))
                    {
                        if (!m_ValidateImportAndExport)
                            continue;

                        throw new Exception($"Cannot find global function  {l_WasmImport.Name}");    
                    }    
                    
                    p_ImportModule.ImportMethod(l_WasmImport.Name, l_Delegate);
                    continue;
                }
                else if (l_WasmImport.Kind == WasmExternalKind.Memory)
                {
                    if (m_ImportedMemoryAreas == null)
                        continue;
                    
                    if (!m_ImportedMemoryAreas.TryGetValue(l_WasmImport.Name, out IWebAssemblyMemoryArea l_Memory))
                    {
                        if (!m_ValidateImportAndExport)
                            continue;

                        throw new Exception($"Cannot find global memory {l_WasmImport.Name}");
                    }
                    
                    p_ImportModule.ImportMemoryArea(l_WasmImport.Name, l_Memory);
                    continue;
                } 
                else
                {
                    if (!m_ValidateImportAndExport)
                        continue;

                    throw new Exception($"Cannot find global import {l_WasmImport.Name} of kind {l_WasmImport.Kind}");
                }
                
            }

            if (!m_Modules.TryGetValue(l_WasmImport.Module, out WebAssemblyModuleBuilder l_ExportModule))
            {
                if (!m_ValidateImportAndExport)
                    continue;

                throw new Exception($"Cannot find module {l_WasmImport.Module}");
            }

            WasmExport l_Export = l_ExportModule.MetaData.Export.FirstOrDefault(x => x.Name == l_WasmImport.Name);

            if (l_Export == null)
            {
                throw new Exception($"Cannot find export {l_WasmImport.Name} in module {l_WasmImport.Module}");
            }

            if (l_Export.Kind != l_WasmImport.Kind)
            {
                throw new Exception(
                    $"Export {l_WasmImport.Name} in module {l_WasmImport.Module} has different kind. Expected {l_WasmImport.Kind}, got {l_Export.Kind}");
            }

            switch (l_Export.Kind)
            {
                case WasmExternalKind.Function:
                    IWebAssemblyMethod l_Method = l_ExportModule.GetMethod(l_Export.Name);
                    p_ImportModule.ImportMethod(l_WasmImport.Name, l_Method.DynamicInvoke);
                    break;
                case WasmExternalKind.Memory:
                    IWebAssemblyMemoryArea l_Memory = l_ExportModule.GetMemoryArea(l_Export.Name);
                    p_ImportModule.ImportMemoryArea(l_WasmImport.Name, l_Memory);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(l_Export.Kind));
            }
        }
    }

    public void ImportMethod(string p_Name, Delegate p_Delegate)
    {
        if (m_ImportedMethods == null)
        {
            m_ImportedMethods = new Dictionary<string, Delegate>();
        }
        
        m_ImportedMethods.Add(p_Name, p_Delegate);
    }

    public void ImportMemoryArea(string p_Name, IWebAssemblyMemoryArea p_Memory)
    {
        if (m_ImportedMemoryAreas == null)
            m_ImportedMemoryAreas = new Dictionary<string, IWebAssemblyMemoryArea>();
        
        m_ImportedMemoryAreas.Add(p_Name, p_Memory);
    }
}