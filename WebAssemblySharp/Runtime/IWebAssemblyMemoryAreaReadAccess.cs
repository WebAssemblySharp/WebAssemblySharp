using System;

namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyMemoryAreaReadAccess
{
    Span<byte> GetMemoryAccess(long p_Address, int p_Length);
    
    int GetSize();
}