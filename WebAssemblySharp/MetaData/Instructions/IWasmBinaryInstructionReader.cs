using System;

namespace WebAssemblySharp.MetaData.Instructions;

public interface IWasmBinaryInstructionReader
{
    ulong? ReadLEB128UInt();
    long? ReadLEB128Int();
    ReadOnlySpan<byte> ReadReadBytes(int p_Length);
    
}