using System;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

/*
 * Represents a WebAssembly method that can be invoked at runtime.
 *
 * This interface provides a standardized way to call WebAssembly functions from C#.
 * The implementation handles the marshalling of arguments between the .NET runtime
 * and the WebAssembly runtime.
 *
 * Methods:
 *   Invoke - Executes the WebAssembly method with the provided arguments and returns
 *            the result asynchronously.
 */
public interface IWebAssemblyMethod
{
    ValueTask<object> DynamicInvoke(params object[] p_Args);

    WasmFuncType GetMetaData();

    Func<ValueTask<TResult>> GetDelegate<TResult>() where TResult : struct
    {
        throw new NotSupportedException();
    }
    
    Func<TInput1, ValueTask<TResult>> GetDelegate<TInput1, TResult>() where TInput1 : struct where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TResult>()
        where TInput1 : struct where TResult : struct where TInput2 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TResult>() where TInput1 : struct where TInput2 : struct where TInput3 : struct where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TResult>()
        where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
            ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
            TResult>()
        where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15,
            ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
            TInput15, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
        where TInput15 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15,
            TInput16, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
            TInput15, TInput16, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
        where TInput15 : struct
        where TInput16 : struct
        where TResult : struct
    {
        throw new NotSupportedException();
    }
    
    Func<ValueTask> GetVoidDelegate()
    {
        throw new NotSupportedException();
    }
    
    Func<TInput1, ValueTask> GetVoidDelegate<TInput1>() where TInput1 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, ValueTask> GetVoidDelegate<TInput1, TInput2>() where TInput1 : struct where TInput2 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3>()
        where TInput1 : struct where TInput2 : struct where TInput3 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5>()
        where TInput1 : struct where TInput2 : struct where TInput3 : struct where TInput4 : struct where TInput5 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask> GetVoidDelegate<TInput1,
        TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12>()
        where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13>()
        where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
            ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
            TInput13, TInput14>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        TInput15, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
        TInput12, TInput13, TInput14,
        TInput15>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
        where TInput15 : struct
    {
        throw new NotSupportedException();
    }

    Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        TInput15, TInput16, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14,
        TInput15, TInput16>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
        where TInput9 : struct
        where TInput10 : struct
        where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
        where TInput15 : struct
        where TInput16 : struct
    {
        throw new NotSupportedException();
    }
    
}