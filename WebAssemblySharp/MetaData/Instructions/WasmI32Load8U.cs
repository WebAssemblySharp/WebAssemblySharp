namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Load8U: WasmInstruction
{
    public static readonly WasmI32Load8U Instance = new WasmI32Load8U();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Load8U;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}