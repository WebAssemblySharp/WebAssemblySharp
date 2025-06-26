using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.Interpreter;

public class WebAssemblyInterpreterMethod : IWebAssemblyMethod
{
    private readonly WebAssemblyInterpreterVirtualMaschine m_VirtualMachine;
    private readonly WasmFuncType m_FuncType;
    private readonly WasmCode m_Code;
    private readonly string m_Name;
    private readonly Func<object[], ITuple> m_MuliResultCreator;

    public WebAssemblyInterpreterMethod(WebAssemblyInterpreterVirtualMaschine p_VirtualMachine, WasmFuncType p_FuncType, WasmCode p_Code,
        string p_Name)
    {
        m_FuncType = p_FuncType;
        m_Code = p_Code;
        m_Name = p_Name;
        m_VirtualMachine = p_VirtualMachine;
        m_MuliResultCreator = GetMultiResultCreator(p_FuncType.Results);
    }

    private Func<object[], ITuple> GetMultiResultCreator(WasmDataType[] p_FuncTypeResults)
    {
        if (p_FuncTypeResults.Length <= 1)
        {
            // No multi result or single result
            return null;
        }
        
        Type l_ReturnType = WebAssemblyValueTupleUtils.GetValueTupleType(p_FuncTypeResults);
        
        return (args) =>
        {
            return (ITuple)Activator.CreateInstance(l_ReturnType, args);
        };
    }

    public ValueTask<object> DynamicInvoke(params object[] p_Args)
    {
        return DoDynamicInvoke<object>(p_Args);
    }
    
    private async ValueTask<T> DoDynamicInvoke<T>(params object[] p_Args)
    {
        ValidateParameters(p_Args);

        WebAssemblyInterpreterExecutionContext l_Context = m_VirtualMachine.CreateContext(m_FuncType, m_Code, p_Args);
        bool l_ExecuteFrame = await m_VirtualMachine.ExecuteFrame(l_Context, 0);

        if (!l_ExecuteFrame)
            throw new WebAssemblyInterpreterException("Unexpected abort of the frame execution");

        Span<WebAssemblyInterpreterValue> l_JitValues = m_VirtualMachine.FinishContext(l_Context);

        if (l_JitValues.Length == 0)
            return default(T);

        if (l_JitValues.Length == 1)
            return (T)l_JitValues[0].GetRawValue();

        
        object[] l_Results = new object[l_JitValues.Length];

        // Reverse the order of the results because the stack is LIFO
        for (int i = l_JitValues.Length - 1; i >= 0; i--)
        {
            l_Results[l_JitValues.Length - 1 - i] = l_JitValues[i].GetRawValue();
        }

        return (T)m_MuliResultCreator.Invoke(l_Results);
    }

    public WasmFuncType GetMetaData()
    {
        return m_FuncType;
    }

    private void ValidateParameters(object[] p_Args)
    {
        if (p_Args.Length != m_FuncType.Parameters.Length)
            throw new InvalidOperationException("Method " + m_Name + " Invalid number of arguments");

        for (int i = 0; i < p_Args.Length; i++)
        {
            Type l_Type = WebAssemblyDataTypeUtils.GetInternalType(m_FuncType.Parameters[i]);

            if (p_Args[i].GetType() != l_Type)
                throw new InvalidOperationException("Method " + m_Name + " Invalid argument type at index " + i + " expected " +
                                                    m_FuncType.Parameters[i] + " but got " + p_Args[i].GetType());
        }
    }
    
    public Func<ValueTask<TResult>> GetDelegate<TResult>() where TResult : struct
    {
        return () => DoDynamicInvoke<TResult>();
    }

    

    public Func<TInput1, ValueTask<TResult>> GetDelegate<TInput1, TResult>() where TInput1 : struct where TResult : struct
    {
        return (x1) => DoDynamicInvoke<TResult>(x1);
    }

    public Func<TInput1, TInput2, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TResult>()
        where TInput1 : struct where TInput2 : struct where TResult : struct
    {
        return (x1, x2) => DoDynamicInvoke<TResult>(x1, x2);
    }

    public Func<TInput1, TInput2, TInput3, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TResult : struct
    {
        return (x1, x2, x3) => DoDynamicInvoke<TResult>(x1, x2, x3);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TResult>()
        where TInput1 : struct where TInput2 : struct where TInput3 : struct where TInput4 : struct where TResult : struct
    {
        return (x1, x2, x3, x4) => DoDynamicInvoke<TResult>(x1, x2, x3, x4);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TResult>()
        where TInput1 : struct where TInput2 : struct where TInput3 : struct where TInput4 : struct where TInput5 : struct where TResult : struct
    {
        return (x1, x2, x3, x4, x5) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TResult : struct
    {
        return (x1, x2, x3, x4, x5, x6) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TResult>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TResult : struct
    {
        return (x1, x2, x3, x4, x5, x6, x7) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<TResult>>
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
        return (x1, x2, x3, x4, x5, x6, x7, x8) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<TResult>> GetDelegate<TInput1, TInput2,
        TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TResult>() where TInput1 : struct
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
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask<TResult>> GetDelegate<TInput1,
        TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TResult>() where TInput1 : struct
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
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TResult>()
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
        where TResult : struct
    {
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TResult>()
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
        where TResult : struct
    {
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12);
    }

    public
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13,
            ValueTask<TResult>>
        GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13,
            TResult>() where TInput1 : struct
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
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
        TInput13, TInput14,
        TResult>() where TInput1 : struct
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
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        TInput15, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
        TInput12, TInput13, TInput14,
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
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15);
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        TInput15, TInput16, ValueTask<TResult>> GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13, TInput14,
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
        return (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16) => DoDynamicInvoke<TResult>(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12, x13, x14, x15, x16);
    }

    public Func<ValueTask> GetVoidDelegate()
    {
        return new Func<ValueTask>(async () => { await DynamicInvoke(); });
    }

    public Func<TInput1, ValueTask> GetVoidDelegate<TInput1>() where TInput1 : struct
    {
        return new Func<TInput1, ValueTask>(async (x1) => { await DynamicInvoke(x1); });
    }

    public Func<TInput1, TInput2, ValueTask> GetVoidDelegate<TInput1, TInput2>() where TInput1 : struct where TInput2 : struct
    {
        return new Func<TInput1, TInput2, ValueTask>(async (x1, x2) => { await DynamicInvoke(x1, x2); });
    }

    public Func<TInput1, TInput2, TInput3, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3>()
        where TInput1 : struct where TInput2 : struct where TInput3 : struct
    {
        return new Func<TInput1, TInput2, TInput3, ValueTask>(async (x1, x2, x3) => { await DynamicInvoke(x1, x2, x3); });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
    {
        return new Func<TInput1, TInput2, TInput3, TInput4, ValueTask>(async (x1, x2, x3, x4) => { await DynamicInvoke(x1, x2, x3, x4); });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5>()
        where TInput1 : struct where TInput2 : struct where TInput3 : struct where TInput4 : struct where TInput5 : struct
    {
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask>(async (x1, x2, x3, x4, x5) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
    {
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask>(async (x1, x2, x3, x4, x5, x6) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
    {
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask>(async (x1, x2, x3, x4, x5, x6, x7) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask>
        GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8>() where TInput1 : struct
        where TInput2 : struct
        where TInput3 : struct
        where TInput4 : struct
        where TInput5 : struct
        where TInput6 : struct
        where TInput7 : struct
        where TInput8 : struct
    {
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask>(async (x1, x2, x3, x4, x5, x6, x7,
            x8) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask>
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask>(async (x1, x2, x3, x4, x5,
            x6,
            x7,
            x8,
            x9) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask> GetVoidDelegate<TInput1,
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask>(async (x1, x2, x3,
            x4,
            x5,
            x6,
            x7,
            x8,
            x9,
            x10) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask>
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask>(async (x1,
            x2,
            x3,
            x4,
            x5,
            x6,
            x7,
            x8,
            x9,
            x10,
            x11) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask>
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
            ValueTask>(async (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9, x10,
                x11,
                x12);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask>
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
            TInput13,
            ValueTask>(async (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10,
            x11,
            x12,
            x13) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9,
                x10,
                x11,
                x12,
                x13);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
        TInput13,
        TInput14>() where TInput1 : struct
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
            TInput13,
            TInput14,
            ValueTask>(async (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10,
            x11,
            x12,
            x13,
            x14) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9,
                x10,
                x11,
                x12,
                x13,
                x14);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        TInput15, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11,
        TInput12, TInput13,
        TInput14, TInput15>() where TInput1 : struct
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
            TInput13,
            TInput14,
            TInput15,
            ValueTask>(async (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10,
            x11,
            x12,
            x13,
            x14,
            x15) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9,
                x10,
                x11,
                x12,
                x13,
                x14,
                x15);
        });
    }

    public Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14,
        TInput15, TInput16, ValueTask> GetVoidDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10,
        TInput11, TInput12, TInput13,
        TInput14, TInput15, TInput16>() where TInput1 : struct
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
        return new Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12,
            TInput13,
            TInput14,
            TInput15,
            TInput16,
            ValueTask>(async (x1, x2, x3, x4, x5, x6, x7, x8, x9, x10,
            x11,
            x12,
            x13,
            x14,
            x15,
            x16) =>
        {
            await DynamicInvoke(x1, x2, x3, x4, x5, x6, x7, x8, x9,
                x10,
                x11,
                x12,
                x13,
                x14,
                x15,
                x16);
        });
    }
}