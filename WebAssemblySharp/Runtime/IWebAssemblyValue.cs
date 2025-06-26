using System;
using System.Threading.Tasks;

namespace WebAssemblySharp.Runtime;

public interface IWebAssemblyValue
{
    ValueTask<IWebAssemblyValue> Populate(IWebAssemblyExecutor p_Executor, String p_Name);
    ValueTask<IWebAssemblyValue> Populate<TInput1>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1) where TInput1: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2) where TInput1: struct where TInput2: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3) where TInput1: struct where TInput2: struct where TInput3: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct where TInput11: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct where TInput11: struct where TInput12: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct where TInput11: struct where TInput12: struct where TInput13: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13, TInput14 p_Input14) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct where TInput11: struct where TInput12: struct where TInput13: struct where TInput14: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13, TInput14 p_Input14, TInput15 p_Input15) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct where TInput11: struct where TInput12: struct where TInput13: struct where TInput14: struct where TInput15: struct;
    ValueTask<IWebAssemblyValue> Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16>(IWebAssemblyExecutor p_Executor, String p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13, TInput14 p_Input14, TInput15 p_Input15, TInput16 p_Input16) where TInput1: struct where TInput2: struct where TInput3: struct where TInput4: struct where TInput5: struct where TInput6: struct where TInput7: struct where TInput8: struct where TInput9: struct where TInput10: struct where TInput11: struct where TInput12: struct where TInput13: struct where TInput14: struct where TInput15: struct where TInput16: struct;
    
}

public interface IWebAssemblyValueGeneric<T>: IWebAssemblyValue where T: struct
{
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate(IWebAssemblyExecutor p_Executor, string p_Name)
    {
        Func<ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<T>();
        T l_Data = await l_Delegate();
        Load(l_Data, p_Executor);
        return this;
    }

    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1)
    {
        Func<TInput1, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, T>();
        T l_Data = await l_Delegate(p_Input1);
        Load(l_Data, p_Executor);
        // Return the current instance as IWebAssemblyValue
        // this is needed because otherwise the compiler will not copy the struct and all the modifications will be lost
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2)
    {
        Func<TInput1, TInput2, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3)
    {
        Func<TInput1, TInput2, TInput3, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4)
    {
        Func<TInput1, TInput2, TInput3, TInput4, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10, p_Input11);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10, p_Input11, p_Input12);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10, p_Input11, p_Input12, p_Input13);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13, TInput14 p_Input14)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10, p_Input11, p_Input12, p_Input13, p_Input14);
        Load(l_Data, p_Executor);
        return this;
    }
    
    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13, TInput14 p_Input14, TInput15 p_Input15)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10, p_Input11, p_Input12, p_Input13, p_Input14, p_Input15);
        Load(l_Data, p_Executor);
        return this;
    }

    async ValueTask<IWebAssemblyValue> IWebAssemblyValue.Populate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16>(IWebAssemblyExecutor p_Executor, string p_Name, TInput1 p_Input1, TInput2 p_Input2, TInput3 p_Input3, TInput4 p_Input4, TInput5 p_Input5, TInput6 p_Input6, TInput7 p_Input7, TInput8 p_Input8, TInput9 p_Input9, TInput10 p_Input10, TInput11 p_Input11, TInput12 p_Input12, TInput13 p_Input13, TInput14 p_Input14, TInput15 p_Input15, TInput16 p_Input16)
    {
        Func<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, ValueTask<T>> l_Delegate = p_Executor.GetMethod(p_Name).GetDelegate<TInput1, TInput2, TInput3, TInput4, TInput5, TInput6, TInput7, TInput8, TInput9, TInput10, TInput11, TInput12, TInput13, TInput14, TInput15, TInput16, T>();
        T l_Data = await l_Delegate(p_Input1, p_Input2, p_Input3, p_Input4, p_Input5, p_Input6, p_Input7, p_Input8, p_Input9, p_Input10, p_Input11, p_Input12, p_Input13, p_Input14, p_Input15, p_Input16);
        Load(l_Data, p_Executor);
        return this;
    }
    
    void Load(T p_Result, IWebAssemblyExecutor p_Executor);
}