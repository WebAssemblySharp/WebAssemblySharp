using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

/*
 * Represents a WebAssembly method that can be invoked at runtime.
 *
 * This interface provides a standardized way to call WebAssembly functions from C#.
 * The implementation handles the marshalling of arguments between the .NET runtime
 * and the WebAssembly runtime.
 *
 * Methods:
 *   Invoke - Executes the WebAssembly method with the provided arguments and returns
 *            the result asynchronously.
 */
public interface IWebAssemblyMethod
{
    Task<object> Invoke(params object[] p_Args);
    
    WasmFuncType GetMetaData();

}