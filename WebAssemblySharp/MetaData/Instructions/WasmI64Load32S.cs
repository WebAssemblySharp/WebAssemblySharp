namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI64Load32S: WasmInstruction
{
    public static readonly WasmI64Load32S Instance = new WasmI64Load32S();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I64Load32S;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}