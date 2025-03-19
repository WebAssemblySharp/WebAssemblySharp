namespace WebAssemblySharp.MetaData.Instructions;

public class WasmI32Const: WasmInstruction
{
    public int Const { get; set; }
    
    protected override WasmOpcode GetOpcode()
    {
        return WasmOpcode.I32Const;
    }

    public override bool ReadInstruction<TReader>(TReader p_Reader)
    {
        long? l_Const = p_Reader.ReadLEB128Int();
        if (l_Const == null)
            return false;
        
        Const = (int)l_Const.Value;
        return true;    
    }
    
    
}