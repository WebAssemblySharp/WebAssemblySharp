using System;
using System.Text;
using WebAssemblySharp.MetaData;
#if NETSTANDARD2_0
using WebAssemblySharp.Polyfills;
#endif

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmStringInComplete: WasmString
{
    private byte[] m_RawValue;
    
    public WasmStringInComplete(long p_Length) : base(String.Empty)
    {
        m_RawValue = new byte[p_Length];
        RawValuePopulatedIndex = 0;
    }

    public long RawValuePopulatedIndex { get; private set; }
    
    public long BytesRemaining
    {
        get
        {
            return m_RawValue.Length - RawValuePopulatedIndex;
        }
    }

    public void Append(ReadOnlySpan<byte> p_Bytes)
    {
#if NETSTANDARD2_0
        p_Bytes.CopyTo(m_RawValue, (int)RawValuePopulatedIndex);  
#else 
        p_Bytes.CopyTo(m_RawValue.AsSpan((int)RawValuePopulatedIndex));
#endif
        RawValuePopulatedIndex += p_Bytes.Length;
        
    }

    public WasmString ToCompleted()
    {
        return new WasmString(Encoding.UTF8.GetString(m_RawValue));
    }
}