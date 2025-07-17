using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

public static class WebAssemblyJITCompilerUtils
{
    [RequiresDynamicCode("WebAssemblyJITCompilerUtils.GetFuncType require dynamic code")]
    public static Type GetSyncFuncType(WasmFuncType p_FuncType)
    {
        Type l_ResultType;

        if (p_FuncType.Results.Length == 0)
        {
            return GetSyncVoidFuncType(p_FuncType);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ResultType = WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]);         
        }
        else
        {
            l_ResultType = WebAssemblyValueTupleUtils.GetValueTupleType(p_FuncType.Results);         
        }
        
        if (p_FuncType.Parameters.Length == 0)
        {
            return typeof(Func<>).MakeGenericType(l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 1)
        {
            return typeof(Func<,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0]), l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 2)
        {
            return typeof(Func<,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 3)
        {
            return typeof(Func<,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , l_ResultType);      
        }
        else if (p_FuncType.Parameters.Length == 4)
        {
            return typeof(Func<,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 5)
        {
            return typeof(Func<,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 6)
        {
            return typeof(Func<,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 7)
        {
            return typeof(Func<,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 8)
        {
            return typeof(Func<,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 9)
        {
            return typeof(Func<,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 10)
        {
            return typeof(Func<,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 11)
        {
            return typeof(Func<,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 12)
        {
            return typeof(Func<,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 13)
        {
            return typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 14)
        {
            return typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 15)
        {
            return typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[14])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 16)
        {
            return typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[14])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[15])
                , l_ResultType);
        }
        else
        {
            throw new Exception("Invalid number of parameters. Max is 16.");       
        }
    }

    [RequiresDynamicCode("WebAssemblyJITCompilerUtils.GetFuncType require dynamic code")]
    private static Type GetSyncVoidFuncType(WasmFuncType p_FuncType)
    {
        
        if (p_FuncType.Parameters.Length == 0)
        {
            return typeof(Action);
        }
        else if (p_FuncType.Parameters.Length == 1)
        {
            return typeof(Action<>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0]));
        }
        else if (p_FuncType.Parameters.Length == 2)
        {
            return typeof(Action<,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1]));
        }
        else if (p_FuncType.Parameters.Length == 3)
        {
            return typeof(Action<,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2]));      
        }
        else if (p_FuncType.Parameters.Length == 4)
        {
            return typeof(Action<,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3]));
        }
        else if (p_FuncType.Parameters.Length == 5)
        {
            return typeof(Action<,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4]));
        }
        else if (p_FuncType.Parameters.Length == 6)
        {
            return typeof(Action<,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5]));
        }
        else if (p_FuncType.Parameters.Length == 7)
        {
            return typeof(Action<,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6]));
        }
        else if (p_FuncType.Parameters.Length == 8)
        {
            return typeof(Action<,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7]));
        }
        else if (p_FuncType.Parameters.Length == 9)
        {
            return typeof(Action<,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8]));
        }
        else if (p_FuncType.Parameters.Length == 10)
        {
            return typeof(Action<,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9]));
        }
        else if (p_FuncType.Parameters.Length == 11)
        {
            return typeof(Action<,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10]));
        }
        else if (p_FuncType.Parameters.Length == 12)
        {
            return typeof(Action<,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11]));
        }
        else if (p_FuncType.Parameters.Length == 13)
        {
            return typeof(Action<,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12]));
        }
        else if (p_FuncType.Parameters.Length == 14)
        {
            return typeof(Action<,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13]));
        }
        else if (p_FuncType.Parameters.Length == 15)
        {
            return typeof(Action<,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[14]));
        }
        else if (p_FuncType.Parameters.Length == 16)
        {
            return typeof(Action<,,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[14])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[15]));
        }
        else
        {
            throw new Exception("Invalid number of parameters. Max is 16.");       
        }    
    }


    [RequiresDynamicCode("WebAssemblyJITCompilerUtils.GetFuncType require dynamic code")]
    public static Type GetAsyncFuncType(WasmFuncType p_FuncType)
    {
        Type l_ResultType;

        if (p_FuncType.Results.Length == 0)
        {
            l_ResultType = typeof(ValueTask);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ResultType = typeof(ValueTask<>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]));         
        }
        else
        {
            l_ResultType = typeof(ValueTask<>).MakeGenericType(WebAssemblyValueTupleUtils.GetValueTupleType(p_FuncType.Results));         
        }
        
        if (p_FuncType.Parameters.Length == 0)
        {
            return typeof(Func<>).MakeGenericType(l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 1)
        {
            return typeof(Func<,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0]), l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 2)
        {
            return typeof(Func<,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 3)
        {
            return typeof(Func<,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , l_ResultType);      
        }
        else if (p_FuncType.Parameters.Length == 4)
        {
            return typeof(Func<,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 5)
        {
            return typeof(Func<,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 6)
        {
            return typeof(Func<,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 7)
        {
            return typeof(Func<,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 8)
        {
            return typeof(Func<,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 9)
        {
            return typeof(Func<,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 10)
        {
            return typeof(Func<,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 11)
        {
            return typeof(Func<,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 12)
        {
            return typeof(Func<,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 13)
        {
            return typeof(Func<,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 14)
        {
            return typeof(Func<,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 15)
        {
            return typeof(Func<,,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[14])
                , l_ResultType);
        }
        else if (p_FuncType.Parameters.Length == 16)
        {
            return typeof(Func<,,,,,,,,,,,,,,,,>).MakeGenericType(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[0])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[1])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[2])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[3])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[4])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[5])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[6])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[7])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[8])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[9])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[10])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[11])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[12])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[13])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[14])
                , WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Parameters[15])
                , l_ResultType);
        }
        else
        {
            throw new Exception("Invalid number of parameters. Max is 16.");       
        }
    }

    public static bool IsAsyncFuncResultType(Type p_ReturnType)
    {
        if (p_ReturnType == null)
        {
            return false;
        }

        return p_ReturnType == typeof(Task) 
               || p_ReturnType == typeof(ValueTask)
               || (p_ReturnType.IsGenericType && 
                   p_ReturnType.GetGenericTypeDefinition() == typeof(ValueTask<>));
    }
}