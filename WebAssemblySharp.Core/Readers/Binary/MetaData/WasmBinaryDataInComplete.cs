using System;
using WebAssemblySharp.MetaData;
#if NETSTANDARD2_0
using WebAssemblySharp.Polyfills;
#endif

namespace WebAssemblySharp.Readers.Binary.MetaData;

public class WasmBinaryDataInComplete: WasmBinaryData
{
    public WasmBinaryDataInComplete(long p_Length) : base(new byte[p_Length])
    {
        RawValuePopulatedIndex = 0;
    }
    
    public long RawValuePopulatedIndex { get; private set; }
    
    public long BytesRemaining
    {
        get
        {
            return m_Data.Length - RawValuePopulatedIndex;
        }
    }

    public void Append(ReadOnlySpan<byte> p_Bytes)
    {
#if NETSTANDARD2_0
        p_Bytes.CopyTo(m_Data, (int)RawValuePopulatedIndex);
#else 
        p_Bytes.CopyTo(m_Data.AsSpan((int)RawValuePopulatedIndex));
#endif
        RawValuePopulatedIndex += p_Bytes.Length;
    }
    
    public WasmBinaryData ToCompleted()
    {
        return new WasmBinaryData(m_Data);
    }
}