namespace WebAssemblySharp.MetaData.Instructions;

public class WasmGlobalGet: WasmInstruction
{
    public uint GlobalIndex { get; set; }
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.GlobalGet;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        ulong? l_GlobalIndex = p_Reader.ReadLEB128UInt();

        if (l_GlobalIndex == null)
            return false;
        
        GlobalIndex = (uint)l_GlobalIndex.Value;
        return true;
    }
}