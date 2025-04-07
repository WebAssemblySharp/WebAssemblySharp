namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI64Store32: WasmInstruction
{
    public static readonly WasmI64Store32 Instance = new WasmI64Store32();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I64Store32;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}