namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Store: WasmInstruction
{
    public long Alignment { get; set; } = -1;
    public long Offset { get; set; } = -1;
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Store;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        if (Alignment == -1)
        {
            ulong? l_Allignment = p_Reader.ReadLEB128UInt();
            
            if (l_Allignment == null)
                return false;
            
            Alignment = (long)l_Allignment.Value;
        }
        
        if (Offset == -1)
        {
            ulong? l_Offset = p_Reader.ReadLEB128UInt();
            
            if (l_Offset == null)
                return false;
            
            Offset = (long)l_Offset.Value;
        }

        return true;
    }
}