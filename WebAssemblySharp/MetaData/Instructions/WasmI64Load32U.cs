namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI64Load32U: WasmInstruction
{
    public static readonly WasmI64Load32U Instance = new WasmI64Load32U();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I64Load32U;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}