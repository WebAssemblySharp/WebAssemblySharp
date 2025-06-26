namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32GeU: WasmInstruction
{
    public static readonly WasmI32GeU Instance = new WasmI32GeU();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32GeU;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}