using System;

namespace WebAssemblySharp.MetaData.Instructions;

public abstract class WasmInstruction
{
    internal Object VmData { get; set; }
    
    public WasmOpcode Opcode
    {
        get
        {
            return GetOpcode();
        }
    }
    
    protected abstract WasmOpcode GetOpcode();
    
    public abstract bool ReadInstruction<TReader>(TReader p_Reader) where TReader : IWasmBinaryInstructionReader, allows ref struct;
    
}