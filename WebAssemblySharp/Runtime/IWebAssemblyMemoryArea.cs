using System;

namespace WebAssemblySharp.Runtime;

/**
 * This interface provides methods to retrieve a span of memory for reading and writing,
 * as well as to get the size of the memory area.
 *
 * Implementations of this interface should provide the actual memory management logic,
 * including allocation, deallocation, and access control.
 */
public interface IWebAssemblyMemoryArea: IWebAssemblyMemoryAreaReadAccess
{
    
    int GetCurrentPages();
    
    int GetMaximumPages();
    
    int GrowMemory(int p_Pages);
    byte[] GetInternalMemory();
}