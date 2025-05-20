namespace WebAssemblySharp.MetaData.Instructions;

public class WasmMemoryFill: WasmInstruction
{
    public int MemoryIndex { get; set; } = -1; 
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.MemoryFill;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        if (MemoryIndex == -1)
        {
            ulong? l_MemoryIndex = p_Reader.ReadLEB128UInt();
            
            if (l_MemoryIndex == null)
                return false;
            
            MemoryIndex = (int)l_MemoryIndex.Value;
        }

        return true;
    }
}