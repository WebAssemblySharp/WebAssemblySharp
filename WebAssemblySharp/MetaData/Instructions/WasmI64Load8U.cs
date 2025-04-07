namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI64Load8U: WasmInstruction
{
    public static readonly WasmI64Load8U Instance = new WasmI64Load8U();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I64Load8U;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}