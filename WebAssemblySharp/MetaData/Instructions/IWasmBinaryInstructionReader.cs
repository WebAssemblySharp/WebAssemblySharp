namespace WebAssemblySharp.MetaData.Instructions;

public interface IWasmBinaryInstructionReader
{
    ulong? ReadLEB128UInt();
}