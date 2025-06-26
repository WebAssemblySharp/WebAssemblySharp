namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Sub: WasmInstruction
{
    public static readonly WasmI32Sub Instance = new WasmI32Sub();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Sub;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}