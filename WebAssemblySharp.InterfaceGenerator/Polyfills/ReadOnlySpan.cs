#if NETSTANDARD2_0
using System;

namespace WebAssemblySharp.Polyfills;

/// <summary>
/// Provides a polyfill implementation of ReadOnlySpan&lt;T&gt; for .NET Standard 2.0 compatibility.
/// This implementation emulates the behavior of the built-in ReadOnlySpan&lt;T&gt; available in newer .NET versions.
/// </summary>
/// <typeparam name="T">The type of elements in the read-only span.</typeparam>
public class ReadOnlySpan<T>
{
    private T[] m_Buffer;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ReadOnlySpan{T}"/> class from a segment of an array.
    /// </summary>
    /// <param name="p_Buffer">The source array.</param>
    /// <param name="p_Index">The starting position within the source array.</param>
    /// <param name="p_Length">The number of elements to include in the read-only span.</param>
    public ReadOnlySpan(T[] p_Buffer, int p_Index, int p_Length)
    {
        m_Buffer = new T[p_Length];

        for (int i = 0; i < p_Length; i++)
        {
            m_Buffer[i] = p_Buffer[p_Index + i];
        }
    }

    /// <summary>
    /// Initializes a new empty instance of the <see cref="ReadOnlySpan{T}"/> class.
    /// </summary>
    public ReadOnlySpan()
    {
        m_Buffer = Array.Empty<T>();
    }

    /// <summary>
    /// Gets a value indicating whether the span is empty.
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            return m_Buffer.Length == 0;
        }
    }

    /// <summary>
    /// Gets the length of the span.
    /// </summary>
    public int Length
    {
        get
        {
            return m_Buffer.Length;
        }
    }
    
    /// <summary>
    /// Gets the element at the specified index in the span.
    /// </summary>
    /// <param name="p_Index">The zero-based index of the element to get.</param>
    /// <returns>The element at the specified index.</returns>
    public T this[int p_Index]
    {
        get
        {
            return m_Buffer[p_Index];
        }
    }

    /// <summary>
    /// Copies the contents of this read-only span into a destination array.
    /// </summary>
    /// <param name="p_Data">The destination array.</param>
    /// <param name="p_Index">The starting index in the destination array.</param>
    public void CopyTo(T[] p_Data, int p_Index)
    {
        m_Buffer.CopyTo(p_Data, p_Index);    
    }

    /// <summary>
    /// Returns the underlying array containing the elements of the span.
    /// </summary>
    /// <returns>The array containing the elements of the span.</returns>
    public T[] GetRaw()
    {
        return m_Buffer;
    }
}


#endif