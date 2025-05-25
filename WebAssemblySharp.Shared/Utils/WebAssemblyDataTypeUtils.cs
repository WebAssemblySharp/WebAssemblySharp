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

    /*
     * Returns the default value for a given WebAssembly data type.
     *
     * Parameters:
     *   p_WasmDataType - The WebAssembly data type to get the default value for.
     *
     * Returns:
     *   The default value as an Object. For numeric types, returns zero values.
     *   For unknown type, returns null.
     *
     * Exceptions:
     *   ArgumentOutOfRangeException - Thrown when the provided WebAssembly data type is not supported.
     */
    public static Object GetDefaultValue(WasmDataType p_WasmDataType)
    {
        switch (p_WasmDataType)
        {
            case WasmDataType.Unkown:
                return null;
            case WasmDataType.I32:
                return 0;
            case WasmDataType.I64:
                return 0L;
            case WasmDataType.F32:
                return 0.0f;
            case WasmDataType.F64:
                return 0.0d;
            default:
                throw new ArgumentOutOfRangeException(nameof(p_WasmDataType), p_WasmDataType, null);
        }
    }
}