namespace WebAssemblySharp.MetaData.Instructions;

public class WasmBr: WasmInstruction
{
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.Br;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        ulong? l_LabelIndex = p_Reader.ReadLEB128UInt();

        if (l_LabelIndex == null)
            return false;
        
        LabelIndex = (uint)l_LabelIndex.Value;
        return true;
    }
    
    public uint LabelIndex { get; set; }
}