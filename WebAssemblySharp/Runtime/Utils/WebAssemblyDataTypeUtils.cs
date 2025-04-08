using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Utils;

/*
 * Utility class for handling WebAssembly data types.
 */
public static class WebAssemblyDataTypeUtils
{
    /*
     * Maps a WebAssembly data type to its corresponding .NET internal type.
     *
     * Parameters:
     *   p_WasmDataType - The WebAssembly data type to map.
     *
     * Returns:
     *   The corresponding .NET type.
     *
     * Exceptions:
     *   ArgumentOutOfRangeException - Thrown when the provided WebAssembly data type is not supported.
     */
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