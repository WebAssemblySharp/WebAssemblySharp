using System;
using System.IO;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Readers.Binary;
using WebAssemblySharp.Runtime.Interpreter;

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
    private Type m_ExecutorType;

    public WebAssemblyRuntime() : this(typeof(WebAssemblyInterpreterExecutor))
    {
    }
    
    public WebAssemblyRuntime(Type p_ExecutorType)
    {
        m_ExecutorType = p_ExecutorType;
    }
    
    
    public async Task<WebAssemblyModuleBuilder> LoadModule(Stream p_Stream)
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
        
        return CreateModule(m_ExecutorType, l_WasmMetaData);
    }

    private WebAssemblyModuleBuilder CreateModule(Type p_RuntimeType, WasmMetaData p_WasmMetaData)
    {
        // Load the code
        IWebAssemblyExecutor l_Executor = (IWebAssemblyExecutor)Activator.CreateInstance(p_RuntimeType);

        // Load the code
        l_Executor.LoadCode(p_WasmMetaData);

        return new WebAssemblyModuleBuilder(l_Executor);
    }
}