namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32RemU: WasmInstruction
{
    public static readonly WasmI32RemU Instance = new WasmI32RemU();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32RemU;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}