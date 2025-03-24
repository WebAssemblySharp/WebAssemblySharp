namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Add: WasmInstruction
{
    public static readonly WasmI32Add Instance = new WasmI32Add();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Add;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}