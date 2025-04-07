namespace WebAssemblySharp.MetaData.Instructions;

public class WasmUnreachable: WasmInstruction
{
    public static readonly WasmUnreachable Instance = new WasmUnreachable();
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.Unreachable;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        return true;
    }
}