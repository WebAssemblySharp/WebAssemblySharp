using System;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITExecutorMethod<TResult> : IWebAssemblyMethod where TResult : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate();
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }
    
    public Func<ValueTask<TResult1>> GetDelegate<TResult1>() where TResult1 : struct
    {
        return (Func<ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TResult> : IWebAssemblyMethod where TResult : struct where TInput1 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, ValueTask<TResult1>> GetDelegate<TInput11, TResult1>() where TInput11 : struct where TResult1 : struct
    {
        return (Func<TInput11, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TResult> : IWebAssemblyMethod
    where TResult : struct where TInput1 : struct where TInput2 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }
    

    public Func<TInput11, TInput21, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TResult1>() where TInput11 : struct where TInput21 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TResult> : IWebAssemblyMethod where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, ValueTask<TResult1>>)(object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TResult> : IWebAssemblyMethod where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TResult> : IWebAssemblyMethod where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TResult> : IWebAssemblyMethod where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
    where TInput6 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        TResult l_Result = await m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2],
            (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5]);
        return l_Result;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TResult> : IWebAssemblyMethod
    where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
    where TInput6 : struct
    where TInput7 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TResult> : IWebAssemblyMethod
    where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
    where TInput6 : struct
    where TInput7 : struct
    where TInput8 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9,
    TResult> : IWebAssemblyMethod
    where TResult : struct
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
    where TInput6 : struct
    where TInput7 : struct
    where TInput8 : struct
    where TInput9 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91,
            ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TResult> : IWebAssemblyMethod where TResult : struct
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
{
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, ValueTask<TResult1>> GetDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TResult1>() where TInput11 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TResult1 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TResult> : IWebAssemblyMethod where TResult : struct
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
{
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput12, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, ValueTask<TResult1>> GetDelegate<TInput12, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TResult1>() where TInput12 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TInput111 : struct where TResult1 : struct
    {
        return (Func<TInput12, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111,
            ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TResult> : IWebAssemblyMethod where TResult : struct
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
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput13, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, ValueTask<TResult1>> GetDelegate<TInput13, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121,
        TResult1>() where TInput13 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TInput111 : struct where TInput121 : struct where TResult1 : struct
    {
        return (Func<TInput13, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101,
            TInput111, TInput121, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TResult> : IWebAssemblyMethod where TResult : struct
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
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput14, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131, ValueTask<TResult1>> GetDelegate<TInput14, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TResult1>() where TInput14 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TInput111 : struct where TInput121 : struct where TInput131 : struct where TResult1 : struct
    {
        return (Func<TInput14, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101,
            TInput111, TInput121, TInput131, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TResult> : IWebAssemblyMethod where TResult : struct
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
{
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput15, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131, TInput141, ValueTask<TResult1>> GetDelegate<TInput15, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TInput141, TResult1>() where TInput15 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TInput111 : struct where TInput121 : struct where TInput131 : struct where TInput141 : struct where TResult1 : struct
    {
        return (Func<TInput15, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101,
            TInput111, TInput121, TInput131, TInput141, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TResult> : IWebAssemblyMethod where TResult : struct
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
    where TInput15 : struct
{
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput16, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131, TInput141, TInput151, ValueTask<TResult1>> GetDelegate<TInput16, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TInput141, TInput151, TResult1>() where TInput16 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TInput111 : struct where TInput121 : struct where TInput131 : struct where TInput141 : struct where TInput151 : struct where TResult1 : struct
    {
        return (Func<TInput16, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101,
            TInput111, TInput121, TInput131, TInput141, TInput151, ValueTask<TResult1>>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8,
    TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, TResult> : IWebAssemblyMethod where TResult : struct
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
    where TInput15 : struct
    where TInput16 : struct
{
    private readonly WasmFuncType m_FuncMetaData;

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask<TResult>> m_Delegate;

    public WebAssemblyJITExecutorMethod(Object p_Target, WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7,
            TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask<TResult>>>(p_Target);
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
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
    
    public Delegate GetNativeDelegate()
    {
        return m_Delegate;
    }

    public Func<TInput17, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131, TInput141, TInput151, TInput161, ValueTask<TResult1>> GetDelegate<TInput17, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TInput141, TInput151, TInput161, TResult1>() where TInput17 : struct where TInput21 : struct where TInput31 : struct where TInput41 : struct where TInput51 : struct where TInput61 : struct where TInput71 : struct where TInput81 : struct where TInput91 : struct where TInput101 : struct where TInput111 : struct where TInput121 : struct where TInput131 : struct where TInput141 : struct where TInput151 : struct where TInput161 : struct where TResult1 : struct
    {
        return (Func<TInput17, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131, TInput141, TInput151, TInput161, ValueTask<TResult1>>)(object)m_Delegate;
    }
}