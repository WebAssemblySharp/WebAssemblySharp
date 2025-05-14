using System;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITExecutorMethod<TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate();
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], 
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
            (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
            (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8], 
                                             (TInput10)p_Args[9]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, 
        TInput11, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8], 
                                             (TInput10)p_Args[9], (TInput11)p_Args[10]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8], 
                                             (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8], 
                                             (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11], 
                                             (TInput13)p_Args[12]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8],
                                             (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
                                             (TInput13)p_Args[12], (TInput14)p_Args[13]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8],
                                             (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
                                             (TInput13)p_Args[12], (TInput14)p_Args[13], (TInput15)p_Args[14]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, TResult> : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask<TResult>>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
                                             (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5],
                                             (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8],
                                             (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11], 
                                             (TInput13)p_Args[12], (TInput14)p_Args[13], (TInput15)p_Args[14],
                                             (TInput16)p_Args[15]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
}