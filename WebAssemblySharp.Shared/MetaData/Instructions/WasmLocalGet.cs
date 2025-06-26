namespace WebAssemblySharp.MetaData.Instructions;

public class WasmLocalGet: WasmInstruction
{
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.LocalGet;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        ulong? l_LocalIndex = p_Reader.ReadLEB128UInt();

        if (l_LocalIndex == null)
            return false;
        
        LocalIndex = (uint)l_LocalIndex.Value;
        return true;
    }


    public uint LocalIndex { get; set; }
    
}