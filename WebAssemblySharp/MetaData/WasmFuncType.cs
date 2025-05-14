using System.Linq;

namespace WebAssemblySharp.MetaData;

public class WasmFuncType
{
    public WasmDataType[] Parameters { get; set; }
    public WasmDataType[] Results { get; set; }

    public override string ToString()
    {
        if ((Parameters == null || Parameters.Length == 0) && (Results == null || Results.Length == 0))
        {
            return "(): void";
        }
        else if (Parameters == null || Parameters.Length == 0)
        {
            return $"(): ({string.Join(", ", Results)})";
        }
        else if (Results == null || Results.Length == 0)
        {
            return $"({string.Join(", ", Parameters)}): void";
        }
        else
        {
            return $"({string.Join(", ", Parameters)}): ({string.Join(", ", Results)})";
        }
    }
    
    public bool IsSame(WasmFuncType p_FuncType)
    {
        if (Parameters.Length != p_FuncType.Parameters.Length)
            return false;
        
        if (Results.Length != p_FuncType.Results.Length)
            return false;
        
        if (!Parameters.SequenceEqual(p_FuncType.Parameters))
            return false;
        
        if (!Results.SequenceEqual(p_FuncType.Results))
            return false;
        
        return true;
    }
}