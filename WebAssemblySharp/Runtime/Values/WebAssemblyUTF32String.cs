using System;
using System.Text;

namespace WebAssemblySharp.Runtime.Values;

public struct WebAssemblyUTF32String: IWebAssemblyValue
{
    private String m_Value;
    
    public void Load(object p_Result, IWebAssemblyExecutor p_Executor)
    {
        object[] l_Objects = (Object[])p_Result;
        Span<byte> l_Access = p_Executor.GetMemoryArea().GetMemoryAccess((int)l_Objects[1], (int)l_Objects[0]);
        m_Value = Encoding.UTF32.GetString(l_Access);
    }

    public override string ToString()
    {
        return m_Value;
    }
    
    public static implicit operator string(WebAssemblyUTF32String p_Value)
    {
        return p_Value.m_Value;
    }
}