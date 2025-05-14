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
    
    public override bool Equals(object p_Obj)
    {
        if (p_Obj is null) return false;
        if (ReferenceEquals(this, p_Obj)) return true;
        if (p_Obj.GetType() != GetType()) return false;
        return m_RawValue == ((WasmString)p_Obj).m_RawValue;
    }

    public override int GetHashCode()
    {
        return (m_RawValue != null ? m_RawValue.GetHashCode() : 0);
    }

    public static bool Equals(WasmString p_A, WasmString p_B)
    {
        return Object.Equals(p_A, p_B);
    }
    
    public static bool operator ==(WasmString p_A, WasmString p_B)
    {
        return Equals(p_A, p_B);
    }

    public static bool operator !=(WasmString p_A, WasmString p_B)
    {
        return !Equals(p_A, p_B);
    }
}