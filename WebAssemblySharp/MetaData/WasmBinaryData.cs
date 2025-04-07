namespace WebAssemblySharp.MetaData;

public class WasmBinaryData
{
    protected byte[] m_Data;
    
    public WasmBinaryData(byte[] p_Data)
    {
        m_Data = p_Data;
    }

    public byte[] Data
    {
        get { return m_Data; }
    }
}