using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime;

[SuppressMessage("Warning", "IL3050")]
[SuppressMessage("Warning", "IL2060")]
public class WebAssemblyModuleDispatchProxy : DispatchProxy
{
    private static MethodInfo DispatchWithReflectionMethodeInfo = typeof(WebAssemblyModuleDispatchProxy).GetMethod("DispatchWithReflection", BindingFlags.NonPublic | BindingFlags.Static);
    
    private WebAssemblyModule m_WebAssemblyModule;

    protected override object Invoke(MethodInfo p_TargetMethod, object[] p_Args)
    {
        IWebAssemblyMethod l_Method = m_WebAssemblyModule.GetMethod(p_TargetMethod.Name);

        Type l_ReturnType = p_TargetMethod.ReturnType;

        if (l_ReturnType == typeof(ValueTask))
        {
            ValueTask<object> l_Task = l_Method.DynamicInvoke(p_Args);
            
            if (l_Task.IsCompleted)
            {
                return ValueTask.CompletedTask;
            }
            else
            {
                TaskCompletionSource l_TaskCompletionSource = new TaskCompletionSource();
                
                Task.Run(async void () =>
                {
                    try
                    {
                        await l_Task;
                        l_TaskCompletionSource.SetResult();
                    }
                    catch (Exception e)
                    {
                        l_TaskCompletionSource.SetException(e);
                    }
                });
                
                return l_TaskCompletionSource.Task;
            }
        }
        else if (l_ReturnType == typeof(ValueTask<int>))
        {
            ValueTask<object> l_Task = l_Method.DynamicInvoke(p_Args);
            
            if (l_Task.IsCompleted)
            {
                object l_Result = l_Task.Result;
                return ValueTask.FromResult<int>((int)l_Result);
            }
            else
            {
                TaskCompletionSource<int> l_TaskCompletionSource = new TaskCompletionSource<int>();
                
                Task.Run(async void () =>
                {
                    try
                    {
                        object l_Result = await l_Task;
                        l_TaskCompletionSource.SetResult((int)l_Result);
                    }
                    catch (Exception e)
                    {
                        l_TaskCompletionSource.SetException(e);
                    }
                });
                
                return l_TaskCompletionSource.Task;
            }
        }
        else if (l_ReturnType == typeof(ValueTask<long>))
        {
            ValueTask<object> l_Task = l_Method.DynamicInvoke(p_Args);
            
            if (l_Task.IsCompleted)
            {
                object l_Result = l_Task.Result;
                return ValueTask.FromResult<long>((long)l_Result);
            }
            else
            {
                TaskCompletionSource<long> l_TaskCompletionSource = new TaskCompletionSource<long>();
                
                Task.Run(async void () =>
                {
                    try
                    {
                        object l_Result = await l_Task;
                        l_TaskCompletionSource.SetResult((long)l_Result);
                    }
                    catch (Exception e)
                    {
                        l_TaskCompletionSource.SetException(e);
                    }
                });
                
                return l_TaskCompletionSource.Task;
            }
        }
        else if (l_ReturnType == typeof(ValueTask<float>))
        {
            ValueTask<object> l_Task = l_Method.DynamicInvoke(p_Args);
            
            if (l_Task.IsCompleted)
            {
                object l_Result = l_Task.Result;
                return ValueTask.FromResult<float>((float)l_Result);
            }
            else
            {
                TaskCompletionSource<float> l_TaskCompletionSource = new TaskCompletionSource<float>();
                
                Task.Run(async void () =>
                {
                    try
                    {
                        object l_Result = await l_Task;
                        l_TaskCompletionSource.SetResult((float)l_Result);
                    }
                    catch (Exception e)
                    {
                        l_TaskCompletionSource.SetException(e);
                    }
                });
                
                return l_TaskCompletionSource.Task;
            }     
        }
        else if (l_ReturnType == typeof(ValueTask<double>))
        {
            ValueTask<object> l_Task = l_Method.DynamicInvoke(p_Args);
            
            if (l_Task.IsCompleted)
            {
                object l_Result = l_Task.Result;
                return ValueTask.FromResult<double>((double)l_Result);
            }
            else
            {
                TaskCompletionSource<double> l_TaskCompletionSource = new TaskCompletionSource<double>();
                
                Task.Run(async void () =>
                {
                    try
                    {
                        object l_Result = await l_Task;
                        l_TaskCompletionSource.SetResult((double)l_Result);
                    }
                    catch (Exception e)
                    {
                        l_TaskCompletionSource.SetException(e);
                    }
                });
                
                return l_TaskCompletionSource.Task;
            }       
        }
        else if (l_ReturnType.IsGenericType && l_ReturnType.GetGenericArguments()[0].IsAssignableTo(typeof(ITuple)))
        {
            ValueTask<object> l_Task = l_Method.DynamicInvoke(p_Args); 
            return DispatchWithReflectionMethodeInfo.MakeGenericMethod(l_ReturnType.GetGenericArguments()[0]).Invoke(null, new object[] { l_Task });
        }
        else
        {
            throw new Exception(
                $"Unsupported return type '{l_ReturnType.FullName}' for method '{p_TargetMethod.Name}' in WebAssembly module.");
        }
    }

    public void Init(WebAssemblyModule p_WebAssemblyModule)
    {
        m_WebAssemblyModule = p_WebAssemblyModule;
    }

    static object DispatchWithReflection<T>(ValueTask<object> p_Task)
    {
        if (p_Task.IsCompleted)
        {
            object l_Result = p_Task.Result;
            return ValueTask.FromResult<T>((T)l_Result);
        }
        else
        {
            TaskCompletionSource<T> l_TaskCompletionSource = new TaskCompletionSource<T>();
                
            Task.Run(async void () =>
            {
                try
                {
                    object l_Result = await p_Task;
                    l_TaskCompletionSource.SetResult((T)l_Result);
                }
                catch (Exception e)
                {
                    l_TaskCompletionSource.SetException(e);
                }
            });
                
            return l_TaskCompletionSource.Task;
        }  
    } 
    
}