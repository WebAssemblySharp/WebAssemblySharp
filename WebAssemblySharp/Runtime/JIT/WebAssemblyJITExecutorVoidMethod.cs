using System;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITExecutorVoidMethod : IWebAssemblyMethod
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate();
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<ValueTask> GetVoidDelegate()
    {
        return (Func<ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1> : IWebAssemblyMethod
    where TInput1 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, ValueTask> GetVoidDelegate<TInput11>() where TInput11 : struct
    {
        return (Func<TInput11, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2> : IWebAssemblyMethod
    where TInput1 : struct
    where TInput2 : struct


{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput12, ValueTask> GetVoidDelegate<TInput11, TInput12>() where TInput11 : struct where TInput12 : struct
    {
        return (Func<TInput11, TInput12, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3> : IWebAssemblyMethod
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput12, TInput13, ValueTask> GetVoidDelegate<TInput11, TInput12, TInput13>()
        where TInput11 : struct where TInput12 : struct where TInput13 : struct
    {
        return (Func<TInput11, TInput12, TInput13, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4> : IWebAssemblyMethod
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct


{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput12, TInput13, TInput14, ValueTask> GetVoidDelegate<TInput11, TInput12, TInput13, TInput14>() where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
    {
        return (Func<TInput11, TInput12, TInput13, TInput14, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5> : IWebAssemblyMethod
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct


{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask> GetVoidDelegate<TInput11, TInput12, TInput13, TInput14, TInput15>()
        where TInput11 : struct where TInput12 : struct where TInput13 : struct where TInput14 : struct where TInput15 : struct
    {
        return (Func<TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6> : IWebAssemblyMethod
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
    where TInput6 : struct

{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4],
            (TInput6)p_Args[5]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask>
        GetVoidDelegate<TInput11, TInput12, TInput13, TInput14, TInput15, TInput16>() where TInput11 : struct
        where TInput12 : struct
        where TInput13 : struct
        where TInput14 : struct
        where TInput15 : struct
        where TInput16 : struct
    {
        return (Func<TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7> : IWebAssemblyMethod
    where TInput1 : struct
    where TInput2 : struct
    where TInput3 : struct
    where TInput4 : struct
    where TInput5 : struct
    where TInput6 : struct
    where TInput7 : struct

{
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4],
            (TInput6)p_Args[5], (TInput7)p_Args[6]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, ValueTask>
        GetVoidDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71>() where TInput11 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8> : IWebAssemblyMethod
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
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4],
            (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, ValueTask> GetVoidDelegate<TInput11, TInput21,
        TInput31, TInput41, TInput51, TInput61, TInput71, TInput81>() where TInput11 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9> : IWebAssemblyMethod
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
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4],
            (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, ValueTask> GetVoidDelegate<TInput11,
        TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91>() where TInput11 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9,
    TInput10> : IWebAssemblyMethod
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
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4],
            (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8], (TInput10)p_Args[9]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, ValueTask>
        GetVoidDelegate<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101>() where TInput11 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
    {
        return (Func<TInput11, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, ValueTask>)
            (Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
    TInput11> : IWebAssemblyMethod
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
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput12, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, ValueTask>
        GetVoidDelegate<TInput12, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111>()
        where TInput12 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
        where TInput111 : struct
    {
        return (Func<TInput12, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, ValueTask>)
            (Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
    TInput12> : IWebAssemblyMethod
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

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask>
        m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
                ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput13, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, ValueTask>
        GetVoidDelegate<TInput13, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121>()
        where TInput13 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
        where TInput111 : struct
        where TInput121 : struct
    {
        return (Func<TInput13, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121,
            ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
    TInput12, TInput13> : IWebAssemblyMethod
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

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13,
        ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
                TInput13, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput14, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        ValueTask> GetVoidDelegate<TInput14, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111,
        TInput121,
        TInput131>() where TInput14 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
        where TInput111 : struct
        where TInput121 : struct
        where TInput131 : struct
    {
        return (Func<TInput14, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121,
            TInput131, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
    TInput12, TInput13, TInput14> : IWebAssemblyMethod
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

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13,
        TInput14, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
                TInput13, TInput14, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12], (TInput14)p_Args[13]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput15, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TInput141, ValueTask> GetVoidDelegate<TInput15, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101,
        TInput111, TInput121,
        TInput131, TInput141>() where TInput15 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
        where TInput111 : struct
        where TInput121 : struct
        where TInput131 : struct
        where TInput141 : struct
    {
        return (Func<TInput15, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121,
            TInput131, TInput141, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
    TInput12, TInput13, TInput14, TInput15> : IWebAssemblyMethod
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

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13,
        TInput14, TInput15, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
                TInput13, TInput14, TInput15, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12], (TInput14)p_Args[13], (TInput15)p_Args[14]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput16, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TInput141, TInput151, ValueTask> GetVoidDelegate<TInput16, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91,
        TInput101, TInput111, TInput121,
        TInput131, TInput141, TInput151>() where TInput16 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
        where TInput111 : struct
        where TInput121 : struct
        where TInput131 : struct
        where TInput141 : struct
        where TInput151 : struct
    {
        return (Func<TInput16, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121,
            TInput131, TInput141, TInput151, ValueTask>)(Object)m_Delegate;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
    TInput12, TInput13, TInput14, TInput15, TInput16> : IWebAssemblyMethod
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

    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13,
        TInput14, TInput15, TInput16, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate)
    {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate
            .CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
                TInput13, TInput14, TInput15, TInput16, ValueTask>>();
    }

    public async ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12], (TInput14)p_Args[13], (TInput15)p_Args[14], (TInput16)p_Args[15]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncMetaData;
    }

    public Func<TInput17, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121, TInput131,
        TInput141, TInput151, TInput161, ValueTask> GetVoidDelegate<TInput17, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81,
        TInput91, TInput101, TInput111, TInput121,
        TInput131, TInput141, TInput151, TInput161>() where TInput17 : struct
        where TInput21 : struct
        where TInput31 : struct
        where TInput41 : struct
        where TInput51 : struct
        where TInput61 : struct
        where TInput71 : struct
        where TInput81 : struct
        where TInput91 : struct
        where TInput101 : struct
        where TInput111 : struct
        where TInput121 : struct
        where TInput131 : struct
        where TInput141 : struct
        where TInput151 : struct
        where TInput161 : struct
    {
        return (Func<TInput17, TInput21, TInput31, TInput41, TInput51, TInput61, TInput71, TInput81, TInput91, TInput101, TInput111, TInput121,
            TInput131, TInput141, TInput151, TInput161, ValueTask>)(Object)m_Delegate;
    }
}