namespace WebAssemblySharp.MetaData.Instructions;

public class WasmCall: WasmInstruction
{
    public uint FunctionIndex { get; set; }
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.Call;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        ulong? l_FunctionIndex = p_Reader.ReadLEB128UInt();

        if (l_FunctionIndex == null)
            return false;
        
        FunctionIndex = (uint)l_FunctionIndex.Value;
        return true;
    }
}