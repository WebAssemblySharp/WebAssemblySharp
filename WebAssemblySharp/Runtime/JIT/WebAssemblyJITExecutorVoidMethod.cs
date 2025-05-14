using System;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITExecutorVoidMethod : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate();
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

// Continue this pattern for classes from 3 to 16 input parameters:

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate((TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3], (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7], (TInput9)p_Args[8], (TInput10)p_Args[9]);
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12], (TInput14)p_Args[13]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12], (TInput14)p_Args[13], (TInput15)p_Args[14]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}

public class WebAssemblyJITExecutorVoidMethod<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16> : IWebAssemblyMethod {
    private readonly WasmFuncType m_FuncMetaData;
    private readonly Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask> m_Delegate;

    public WebAssemblyJITExecutorVoidMethod(WasmFuncType p_FuncMetaData, MethodInfo p_Delegate) {
        m_FuncMetaData = p_FuncMetaData;
        m_Delegate = p_Delegate.CreateDelegate<Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask>>();
    }

    public async ValueTask<object> Invoke(params object[] p_Args) {
        ValueTask l_Task = m_Delegate(
            (TInput1)p_Args[0], (TInput2)p_Args[1], (TInput3)p_Args[2], (TInput4)p_Args[3],
            (TInput5)p_Args[4], (TInput6)p_Args[5], (TInput7)p_Args[6], (TInput8)p_Args[7],
            (TInput9)p_Args[8], (TInput10)p_Args[9], (TInput11)p_Args[10], (TInput12)p_Args[11],
            (TInput13)p_Args[12], (TInput14)p_Args[13], (TInput15)p_Args[14], (TInput16)p_Args[15]
        );
        await l_Task;
        return null;
    }

    public WasmFuncType GetMetaData() {
        return m_FuncMetaData;
    }
}