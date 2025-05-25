namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Ne: WasmInstruction
{
    public static readonly WasmI32Ne Instance = new WasmI32Ne();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Ne;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}