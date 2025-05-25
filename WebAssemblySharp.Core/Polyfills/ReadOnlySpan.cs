#if NETSTANDARD2_0
using System;

namespace WebAssemblySharp.Polyfills;

public class ReadOnlySpan<T>
{
    private T[] m_Buffer;
    
    public ReadOnlySpan(T[] p_Buffer, int p_Index, int p_Length)
    {
        m_Buffer = new T[p_Length];

        for (int i = 0; i < p_Length; i++)
        {
            m_Buffer[i] = p_Buffer[p_Index + i];
        }
    }

    public ReadOnlySpan()
    {
        m_Buffer = Array.Empty<T>();
    }

    public bool IsEmpty
    {
        get
        {
            return m_Buffer.Length == 0;
        }
    }

    public int Length
    {
        get
        {
            return m_Buffer.Length;
        }
    }
    
    public T this[int p_Index]
    {
        get
        {
            return m_Buffer[p_Index];
        }
    }

    public void CopyTo(T[] p_Data, int p_Index)
    {
        m_Buffer.CopyTo(p_Data, p_Index);    
    }

    public T[] GetRaw()
    {
        return m_Buffer;
    }
}


#endif