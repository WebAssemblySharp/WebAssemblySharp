using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Interpreter;

public struct WebAssemblyInterpreterValue
{
    // MetaDat
    private readonly WasmDataType m_DataType;
    private readonly bool m_Mutable;
    
    // Values
    private int m_IntValue;
    private long m_LongValue;
    private float m_FloatValue;
    private double m_DoubleValue;

    public static WebAssemblyInterpreterValue CreateDynamic(WasmDataType p_DataType, Object p_Value)
    {
        switch (p_DataType)
        {
            case WasmDataType.I32:
                return new WebAssemblyInterpreterValue((int)p_Value);
            case WasmDataType.I64:
                return new WebAssemblyInterpreterValue((long)p_Value);
            case WasmDataType.F32:
                return new WebAssemblyInterpreterValue((float)p_Value);
            case WasmDataType.F64:
                return new WebAssemblyInterpreterValue((double)p_Value);
            default:
                throw new ArgumentOutOfRangeException(nameof(p_DataType), p_DataType, null);
        }
    }
    
    public WebAssemblyInterpreterValue(int p_IntValue)
    {
        m_DataType = WasmDataType.I32;
        m_Mutable = false;
        m_IntValue = p_IntValue;
    }
    
    public WebAssemblyInterpreterValue(long p_LongValue)
    {
        m_DataType = WasmDataType.I64;
        m_Mutable = false;
        m_LongValue = p_LongValue;
    }
    
    public WebAssemblyInterpreterValue(float p_FloatValue)
    {
        m_DataType = WasmDataType.F32;
        m_Mutable = false;
        m_FloatValue = p_FloatValue;
    }

    public WebAssemblyInterpreterValue(double p_DoubleValue)
    {
        m_DataType = WasmDataType.F64;
        m_Mutable = false;
        m_DoubleValue = p_DoubleValue;
    }

    public WebAssemblyInterpreterValue(int p_IntValue, bool p_Mutable)
    {
        m_DataType = WasmDataType.I32;
        m_IntValue = p_IntValue;
        m_Mutable = p_Mutable;
    }
    
    public WebAssemblyInterpreterValue(long p_LongValue, bool p_Mutable)
    {
        m_DataType = WasmDataType.I64;
        m_LongValue = p_LongValue;
        m_Mutable = p_Mutable;
    }
    
    public WebAssemblyInterpreterValue(float p_FloatValue, bool p_Mutable)
    {
        m_DataType = WasmDataType.F32;
        m_FloatValue = p_FloatValue;
        m_Mutable = p_Mutable;
    }
    
    public WebAssemblyInterpreterValue(double p_DoubleValue, bool p_Mutable)
    {
        m_DataType = WasmDataType.F64;
        m_DoubleValue = p_DoubleValue;
        m_Mutable = p_Mutable;
    }

    public WebAssemblyInterpreterValue(WebAssemblyInterpreterValue p_ValuesAssemblyInterpreterValue, bool p_Mutable)
    {
        m_DataType = p_ValuesAssemblyInterpreterValue.m_DataType;
        m_IntValue = p_ValuesAssemblyInterpreterValue.m_IntValue;
        m_LongValue = p_ValuesAssemblyInterpreterValue.m_LongValue;
        m_FloatValue = p_ValuesAssemblyInterpreterValue.m_FloatValue;
        m_DoubleValue = p_ValuesAssemblyInterpreterValue.m_DoubleValue;
        m_Mutable = p_Mutable;
    }

    public WasmDataType DataType
    {
        get
        {
            return m_DataType;
        }
    }

    public int IntValue
    {
        get
        {
            if (m_DataType != WasmDataType.I32)
                throw new InvalidOperationException($"Invalid data type. Expected {WasmDataType.I32} but got {m_DataType}");
            
            return m_IntValue;
        }
    }

    public long LongValue
    {
        get
        {
            if (m_DataType != WasmDataType.I64)
                throw new InvalidOperationException($"Invalid data type. Expected {WasmDataType.I64} but got {m_DataType}");
            
            return m_LongValue;
        }
    }

    public float FloatValue
    {
        get
        {
            if (m_DataType != WasmDataType.F32)
                throw new InvalidOperationException($"Invalid data type. Expected {WasmDataType.F32} but got {m_DataType}");
            
            return m_FloatValue;
        }
    }

    public double DoubleValue
    {
        get
        {
            if (m_DataType != WasmDataType.F64)
                throw new InvalidOperationException($"Invalid data type. Expected {WasmDataType.F64} but got {m_DataType}");
            
            return m_DoubleValue;
        }
    }


    public void CopyValueFrom(WebAssemblyInterpreterValue p_Value)
    {
        if (m_DataType != p_Value.DataType)
            throw new Exception($"Invalid data type. Expected {DataType} but got {p_Value.DataType}");
        
        if (!m_Mutable)
            throw new InvalidOperationException("Value is not mutable");

        switch (m_DataType)
        {
            case WasmDataType.I32:
                m_IntValue = p_Value.m_IntValue;
                break;
            case WasmDataType.I64:
                m_LongValue = p_Value.m_LongValue;
                break;
            case WasmDataType.F32:
                m_FloatValue = p_Value.m_FloatValue;
                break;
            case WasmDataType.F64:
                m_DoubleValue = p_Value.m_DoubleValue;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(DataType), DataType, null);
        }
    }

    public override string ToString()
    {
        switch (DataType)
        {
            case WasmDataType.Unkown:
                return $"{nameof(DataType)}: {DataType}";
            case WasmDataType.I32:
                return $"{nameof(DataType)}: {DataType}, {nameof(IntValue)}: {IntValue}";
            case WasmDataType.I64:
                return $"{nameof(DataType)}: {DataType}, {nameof(LongValue)}: {LongValue}";
            case WasmDataType.F32:
                return $"{nameof(DataType)}: {DataType}, {nameof(FloatValue)}: {FloatValue}";
            case WasmDataType.F64:
                return $"{nameof(DataType)}: {DataType}, {nameof(DoubleValue)}: {DoubleValue}";
            default:
                return $"{nameof(DataType)}: {DataType}";
        }
    }

    public object GetRawValue()
    {
        switch (m_DataType)
        {
            case WasmDataType.Unkown:
                return null;
            case WasmDataType.I32:
                return m_IntValue;
            case WasmDataType.I64:
                return m_LongValue;
            case WasmDataType.F32:
                return m_FloatValue;
            case WasmDataType.F64:
                return m_DoubleValue;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
    }
}