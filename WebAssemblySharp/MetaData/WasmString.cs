using System;

namespace WebAssemblySharp.MetaData;

public class WasmString
{
    protected String m_RawValue;
    public WasmString(String p_FinalValue)
    {
        m_RawValue = p_FinalValue;
    }
    
    public String Value
    {
        get
        {
            return m_RawValue;
        }
    }

    public override string ToString()
    {
        return m_RawValue;
    }
    
    public static implicit operator String(WasmString p_Value)
    {
        return p_Value.Value;
    }
}