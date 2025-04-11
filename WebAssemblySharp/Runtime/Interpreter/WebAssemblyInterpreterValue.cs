using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Interpreter;

public struct WebAssemblyInterpreterValue
{
    private readonly WasmDataType m_DataType;
    private object m_Value;
    private bool m_Mutable;
    
    public WebAssemblyInterpreterValue(WasmDataType p_DataType, object p_Value)
    {
        m_DataType = p_DataType;
        m_Value = p_Value;
        m_Mutable = true;
    }
    
    public WebAssemblyInterpreterValue(WasmDataType p_DataType, object p_Value, bool p_Mutable)
    {
        m_DataType = p_DataType;
        m_Value = p_Value;
        m_Mutable = p_Mutable;
    }
    
    public WasmDataType DataType
    {
        get { return m_DataType; }
    }
    
    public object Value
    {
        get { return m_Value; }
        set
        {
            if (!m_Mutable)
                throw new InvalidOperationException("Value is not mutable");
            
            m_Value = value;
        }
    }


    public void CopyValueFrom(WebAssemblyInterpreterValue p_Value)
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