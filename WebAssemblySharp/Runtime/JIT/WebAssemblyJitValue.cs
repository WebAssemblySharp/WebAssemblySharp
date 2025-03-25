using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitValue
{
    private WasmDataType m_DataType;
    private object m_Value;
    
    public WebAssemblyJitValue(WasmDataType p_DataType, object p_Value)
    {
        m_DataType = p_DataType;
        m_Value = p_Value;
    }
    
    public WasmDataType DataType
    {
        get { return m_DataType; }
    }
    
    public object Value
    {
        get { return m_Value; }
        set { m_Value = value; }
    }
    
    
}