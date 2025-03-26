using System;
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


    public void CopyValueFrom(WebAssemblyJitValue p_Value)
    {
        if (DataType != p_Value.DataType)
            throw new Exception($"Invalid data type. Expected {DataType} but got {p_Value.DataType}");
        
        Value = p_Value.Value;
    }

    public override string ToString()
    {
        return $"{nameof(DataType)}: {DataType}, {nameof(Value)}: {Value}";
    }
}