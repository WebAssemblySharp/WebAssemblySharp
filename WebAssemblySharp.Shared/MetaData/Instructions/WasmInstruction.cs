using System;

namespace WebAssemblySharp.MetaData.Instructions;

public abstract class WasmInstruction
{
    public Object VmData { get; set; }
    
    public WasmOpcode Opcode
    {
        get
        {
            return GetOpcode();
        }
    }
    
    protected abstract WasmOpcode GetOpcode();

    public abstract bool ReadInstruction<TReader>(TReader p_Reader) where TReader : IWasmBinaryInstructionReader
#if NETSTANDARD2_0
    ;
#else
    ,allows ref struct;
#endif


}