namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI64Load16U: WasmInstruction
{
    public static readonly WasmI64Load16U Instance = new WasmI64Load16U();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I64Load16U;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}