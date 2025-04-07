namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Store: WasmInstruction
{
    public static readonly WasmI32Store Instance = new WasmI32Store();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Store;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}