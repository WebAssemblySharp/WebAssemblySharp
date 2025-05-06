using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

/*
 * Defines an interface for WebAssembly execution engines.
 *
 * This interface provides the core functionality required to load, optimize,
 * and execute WebAssembly modules. Implementations may include interpreters,
 * JIT compilers, or other execution strategies.
 *
 * The executor is responsible for:
 * - Loading WebAssembly metadata and code
 * - Providing access to exported functions through methods
 * - Managing memory areas and imports
 * - Initializing the execution environment
 */
public interface IWebAssemblyExecutor
{
    /*
     * Gets a WebAssembly method by its export name.
     *
     * @param p_Name The export name of the method to retrieve
     * @return The WebAssembly method interface for invoking the function
     */
    IWebAssemblyMethod GetMethod(string p_Name);

    /*
     * Loads WebAssembly code from metadata into the executor.
     *
     * @param p_WasmMetaData The WebAssembly metadata containing code and module information
     */
    void LoadCode(WasmMetaData p_WasmMetaData);

    /*
     * Optimizes the loaded WebAssembly code for better performance.
     * This should be called after loading code but before execution.
     */
    void OptimizeCode();

    /*
     * Initializes the executor asynchronously.
     * This prepares the execution environment and must be called before methods can be invoked.
     *
     * @return A task representing the asynchronous initialization operation
     */
    Task Init();
    
    /*
     * Gets a WebAssembly memory area by its index.
     *
     * @param p_Index The index of the memory area to retrieve (default is 0)
     * @return The WebAssembly memory area interface
     */
    IWebAssemblyMemoryArea GetMemoryArea(int p_Index = 0);
    
    /*
     * Imports an external memory area into the WebAssembly module.
     *
     * @param p_Name The name to be used for the imported memory area
     * @param p_Memory The memory area to import
     */
    void ImportMemoryArea(string p_Name, IWebAssemblyMemoryArea p_Memory);
    
    /*
     * Imports an external method into the WebAssembly module.
     *
     * @param p_Name The name to be used for the imported method
     * @param p_Delegate The .NET delegate representing the method implementation
     */
    void ImportMethod(string p_Name, Delegate p_Delegate);
}