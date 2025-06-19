using System;
using System.Text;

namespace WebAssemblySharp.Runtime.Values;

public struct WebAssemblyUTF8String: IWebAssemblyValueGeneric<ValueTuple<int, int>>
{
    private String m_Value;
    
    public void Load(ValueTuple<int, int> p_Result, IWebAssemblyExecutor p_Executor)
    {
        Span<byte> l_Access = p_Executor.GetInternalMemoryArea().GetMemoryAccess(p_Result.Item1, p_Result.Item2);
        m_Value = Encoding.UTF8.GetString(l_Access);
    }

    public override string ToString()
    {
        return m_Value;
    }
    
    public static implicit operator string(WebAssemblyUTF8String p_Value)
    {
        return p_Value.m_Value;
    }
}