using System;
using System.Collections.Generic;

namespace WebAssemblySharp.Runtime;

/*
 * Represents the main runtime environment for loading and executing WebAssembly modules.
 *
 * This class serves as an entry point for WebAssembly module loading and instantiation.
 * It supports different execution strategies through executor implementations that
 * conform to the IWebAssemblyExecutor interface (such as interpreter or JIT compilation).
 *
 * The runtime handles the process of:
 * - Loading WASM binary from a stream
 * - Parsing the binary format via WasmBinaryReader
 * - Creating metadata for the module
 * - Initializing the appropriate executor implementation
 * - Building a module instance that can be used to call WASM functions
 */
public class WebAssemblyRuntime
{
    
    private Dictionary<String, WebAssemblyModule> m_Modules;

    public WebAssemblyRuntime(List<WebAssemblyModule> p_Modules)
    {
        m_Modules = new Dictionary<string, WebAssemblyModule>();
        
        foreach (WebAssemblyModule l_AssemblyModule in p_Modules)
        {
            m_Modules.Add(l_AssemblyModule.Name, l_AssemblyModule);
        }
        
    }

    public WebAssemblyModule GetModule(string p_Name)
    {
        return m_Modules[p_Name];
    }
}