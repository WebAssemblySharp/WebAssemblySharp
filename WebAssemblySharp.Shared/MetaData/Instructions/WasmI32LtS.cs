namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32LtS: WasmInstruction
{
    public static readonly WasmI32LtS Instance = new WasmI32LtS();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32LtS;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}