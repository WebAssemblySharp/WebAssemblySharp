using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Utils;

public static class WebAssemblyDataTypeUtils
{
    public static Type GetInternalType(WasmDataType p_WasmDataType)
    {
        switch (p_WasmDataType)
        {
            case WasmDataType.I32:
                return typeof(int);
            case WasmDataType.I64:
                return typeof(long);
            case WasmDataType.F32:
                return typeof(float);
            case WasmDataType.F64:
                return typeof(double);
            default:
                throw new ArgumentOutOfRangeException(nameof(p_WasmDataType), p_WasmDataType, null);
        }
    }
}