using System;
using WebAssemblySharp.MetaData;

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
        p_Bytes.CopyTo(m_Data.AsSpan((int)RawValuePopulatedIndex));
        RawValuePopulatedIndex += p_Bytes.Length;
        
    }
    
    public WasmBinaryData ToCompleted()
    {
        return new WasmBinaryData(m_Data);
    }
}