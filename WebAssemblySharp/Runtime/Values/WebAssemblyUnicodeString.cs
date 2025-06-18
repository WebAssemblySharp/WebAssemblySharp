using System;
using System.Text;

namespace WebAssemblySharp.Runtime.Values;

public struct WebAssemblyUnicodeString: IWebAssemblyValue
{
    private String m_Value;
    
    public void Load(object p_Result, IWebAssemblyValueAccess p_Executor)
    {
        (int, int) l_Tuple = (ValueTuple<int, int>)p_Result;
        Span<byte> l_Access = p_Executor.GetInternalMemoryArea().GetMemoryAccess(l_Tuple.Item1, l_Tuple.Item2);
        m_Value = Encoding.Unicode.GetString(l_Access);
    }

    public override string ToString()
    {
        return m_Value;
    }
    
    public static implicit operator string(WebAssemblyUnicodeString p_Value)
    {
        return p_Value.m_Value;
    }
}