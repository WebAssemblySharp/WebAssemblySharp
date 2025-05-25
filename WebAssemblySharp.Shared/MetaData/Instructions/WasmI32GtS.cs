namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32GtS: WasmInstruction
{
    public static readonly WasmI32GtS Instance = new WasmI32GtS();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32GtS;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}