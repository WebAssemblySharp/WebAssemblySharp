using System;
using System.Diagnostics.CodeAnalysis;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Utils;

[RequiresDynamicCode("WebAssemblyValueTupleUtils requires dynamic code.")]
public static class WebAssemblyValueTupleUtils
{
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
    public static Type GetValueTupleType(WasmDataType[] p_FuncTypeResults)
    {
        if (p_FuncTypeResults.Length == 2)
        {
            return typeof(ValueTuple<,>).MakeGenericType(
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[0]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[1]));
        }
        else if (p_FuncTypeResults.Length == 3)
        {
            return typeof(ValueTuple<,,>).MakeGenericType(
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[0]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[1]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[2]));
        }
        else if (p_FuncTypeResults.Length == 4)
        {
            return typeof(ValueTuple<,,,>).MakeGenericType(
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[0]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[1]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[2]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[3]));
        }
        else if (p_FuncTypeResults.Length == 5)
        {
            return typeof(ValueTuple<,,,,>).MakeGenericType(
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[0]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[1]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[2]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[3]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[4]));
        }
        else if (p_FuncTypeResults.Length == 6)
        {
            return typeof(ValueTuple<,,,,,>).MakeGenericType(
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[0]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[1]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[2]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[3]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[4]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[5]));
        }
        else if (p_FuncTypeResults.Length == 7)
        {
            return typeof(ValueTuple<,,,,,,>).MakeGenericType(
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[0]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[1]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[2]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[3]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[4]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[5]),
                WebAssemblyDataTypeUtils.GetInternalType(p_FuncTypeResults[6]));
        }
        else
        {
            throw new InvalidOperationException("Invalid number of results " + p_FuncTypeResults.Length);
        }
    }
}