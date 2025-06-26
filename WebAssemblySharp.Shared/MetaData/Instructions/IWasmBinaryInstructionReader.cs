#if NETSTANDARD2_0
using WebAssemblySharp.Polyfills;
#else
using System;
#endif

namespace WebAssemblySharp.MetaData.Instructions;

public interface IWasmBinaryInstructionReader
{
    ulong? ReadLEB128UInt();
    long? ReadLEB128Int();
    ReadOnlySpan<byte> ReadReadBytes(int p_Length);
    
}