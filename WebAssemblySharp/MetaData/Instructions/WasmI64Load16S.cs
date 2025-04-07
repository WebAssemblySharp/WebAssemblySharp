namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI64Load16S: WasmInstruction
{
    public static readonly WasmI64Load16S Instance = new WasmI64Load16S();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I64Load16S;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}