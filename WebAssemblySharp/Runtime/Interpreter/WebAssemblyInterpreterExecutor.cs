using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.Interpreter;

/*
 * Implements the WebAssembly interpreter execution strategy.
 *
 * This class is responsible for:
 * - Loading and executing WebAssembly modules through an interpreter approach
 * - Managing exports and providing access to exported WebAssembly functions
 * - Handling imports by mapping .NET delegates to WebAssembly import functions
 * - Optimizing code execution through specialized delegate compilation
 * - Initializing memory, data sections, and global variables
 *
 * The executor works with a virtual machine instance to handle the actual instruction
 * execution and maintains caches of compiled methods to improve performance for
 * repeated function calls. It supports various parameter/return value patterns and
 * offers optimized paths for common scenarios.
 *
 * This implementation focuses on correctness and flexibility rather than maximum
 * performance, making it suitable for development and testing scenarios where
 * runtime compilation (JIT) might be impractical.
 */
public class WebAssemblyInterpreterExecutor : IWebAssemblyExecutor, IWebAssemblyInterpreterInteropOptimizer
{
    private WasmMetaData m_WasmMetaData;
    private ConcurrentDictionary<String, IWebAssemblyMethod> m_ExportMethods;

    private IDictionary<WasmImportFunction, WebAssemblyInterpreterImportMethod> m_ImportMethods;
    private IDictionary<int, IWebAssemblyInterpreterMemoryArea> m_ImportedMemoryAreas;

    private WebAssemblyInterpreterVirtualMaschine m_VirtualMaschine;

    public WebAssemblyInterpreterExecutor()
    {
        m_ExportMethods = new ConcurrentDictionary<String, IWebAssemblyMethod>();
        m_ImportMethods = new Dictionary<WasmImportFunction, WebAssemblyInterpreterImportMethod>();
        m_ImportedMemoryAreas = new Dictionary<int, IWebAssemblyInterpreterMemoryArea>();
        m_VirtualMaschine = new WebAssemblyInterpreterVirtualMaschine();
    }

    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        return GetOrCompileMethod(p_Name);
    }

    public void LoadCode(WasmMetaData p_WasmMetaData)
    {
        m_WasmMetaData = p_WasmMetaData;
    }

    public void OptimizeCode()
    {
        // Freeze Imports
        m_ImportMethods = new ReadOnlyDictionary<WasmImportFunction, WebAssemblyInterpreterImportMethod>(m_ImportMethods);
        m_ImportedMemoryAreas = new ReadOnlyDictionary<int, IWebAssemblyInterpreterMemoryArea>(m_ImportedMemoryAreas);

        m_VirtualMaschine.OptimizeCode(this, m_WasmMetaData);
    }

    public void ImportMemoryArea(string p_Name, IWebAssemblyMemoryArea p_Memory)
    {
        WasmImportMemory l_Import = FindImportByName<WasmImportMemory>(p_Name);

        if (l_Import == null)
            throw new Exception("Import not found: " + p_Name);

        if (p_Memory.GetCurrentPages() < l_Import.Min)
            throw new Exception($"Import memory {p_Name} area too small. Expected min {l_Import.Min} pages, got {p_Memory.GetCurrentPages()} pages");

        if (p_Memory.GetMaximumPages() < l_Import.Max)
            throw new Exception($"Import memory {p_Name} area too small. Expected max {l_Import.Max} pages, got {p_Memory.GetMaximumPages()} pages");
    }

    public void ImportMethod(string p_Name, Delegate p_Delegate)
    {
        WasmImportFunction l_Import = FindImportByName<WasmImportFunction>(p_Name);

        if (l_Import == null)
            throw new Exception("Import not found: " + p_Name);

        WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_Import.FunctionIndex];

        Delegate l_Delegate = CompileImport(l_FuncType, p_Delegate);
        WebAssemblyInterpreterImportMethod l_ImportMethod = new WebAssemblyInterpreterImportMethod(l_Import, l_FuncType, l_Delegate);

        if (!m_ImportMethods.TryAdd(l_Import, l_ImportMethod))
            throw new Exception("Import already defined: " + p_Name);
    }

    public IWebAssemblyMemoryAreaReadAccess GetInternalMemoryArea(int p_Index = 0)
    {
        return m_VirtualMaschine.GetMemoryArea(p_Index);
    }

    public async Task Init()
    {
        if (m_ImportedMemoryAreas.Count > 0)
            throw new Exception("QQQ Imported memory areas not supported");

        m_VirtualMaschine.SetupMemory(m_WasmMetaData.Memory);
        await m_VirtualMaschine.PreloadData(m_WasmMetaData.Data);
        await m_VirtualMaschine.InitGlobals(m_WasmMetaData.Globals);
    }

    public IWebAssemblyMemoryArea GetMemoryArea(string p_Name)
    {
        int? l_ExportIndex = FindExportIndex(p_Name, WasmExternalKind.Memory);

        if (!l_ExportIndex.HasValue)
        {
            throw new Exception("Export not found: " + p_Name);
        }

        return m_VirtualMaschine.GetMemoryArea(l_ExportIndex.Value);
    }

    private Delegate CompileImport(WasmFuncType p_FuncType, Delegate p_Delegate)
    {
        // Link to other WebAssembly Method
        if (p_Delegate.Target is IWebAssemblyMethod)
        {
            return CompileWasmImport(p_FuncType, p_Delegate, (IWebAssemblyMethod)p_Delegate.Target);
        }

        // Link CLR Method

        // Validated Input
        if (p_FuncType.Results.Length > 1)
            throw new Exception("Import with multiple return values not supported");

        ParameterInfo[] l_ParameterInfos = p_Delegate.Method.GetParameters();

        if (p_FuncType.Parameters.Length != l_ParameterInfos.Length)
            throw new Exception("Import parameter count mismatch");

        for (int i = 0; i < p_FuncType.Parameters.Length; i++)
        {
            WasmDataType l_WasmDataType = p_FuncType.Parameters[i];
            Type l_ParameterType = l_ParameterInfos[i].ParameterType;

            if (WebAssemblyDataTypeUtils.GetInternalType(l_WasmDataType) != l_ParameterType)
                throw new Exception("Import parameter type mismatch: " + l_WasmDataType + " != " + l_ParameterType + " Parameter: " +
                                    l_ParameterInfos[i].Name);
        }


        if (p_Delegate.Method.ReturnType.IsGenericType && p_Delegate.Method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            Type l_UnwarppedResultType = p_Delegate.Method.ReturnType.GetGenericArguments()[0];

            if (p_FuncType.Results.Length == 1)
            {
                if (WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]) != l_UnwarppedResultType)
                    throw new Exception("Import return type mismatch: " + p_FuncType.Results[0] + " != " + l_UnwarppedResultType);
            }

            if (l_UnwarppedResultType == typeof(int))
            {
                return CompileImportWithResultTask<int>(p_FuncType, p_Delegate);
            }

            if (l_UnwarppedResultType == typeof(long))
            {
                return CompileImportWithResultTask<long>(p_FuncType, p_Delegate);
            }

            if (l_UnwarppedResultType == typeof(float))
            {
                return CompileImportWithResultTask<float>(p_FuncType, p_Delegate);
            }

            if (l_UnwarppedResultType == typeof(double))
            {
                return CompileImportWithResultTask<double>(p_FuncType, p_Delegate);
            }

            throw new Exception("Unsupported Task return type: " + l_UnwarppedResultType);
        }

        if (p_Delegate.Method.ReturnType == typeof(Task))
        {
            // Optimize for 0-3 parameters and Task<void> return value
            if (p_FuncType.Parameters.Length == 0)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    return (Task)p_Delegate.DynamicInvoke();
                });
            }

            if (p_FuncType.Parameters.Length == 1)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    return (Task)p_Delegate.DynamicInvoke(l_Value.GetRawValue());
                });
            }

            if (p_FuncType.Parameters.Length == 2)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                    return (Task)p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue());
                });
            }

            if (p_FuncType.Parameters.Length == 3)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value3 = p_Context.PopFromStack();
                    return (Task)p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue(), l_Value3.GetRawValue());
                });
            }

            // Dynamic Fallbacks
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
            {
                Object[] l_Args = new Object[p_FuncType.Parameters.Length];
                for (int i = 0; i < l_Args.Length; i++)
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    l_Args[i] = l_Value.GetRawValue();
                }

                return (Task)p_Delegate.DynamicInvoke(l_Args);
            });
        }

        if (p_Delegate.Method.ReturnType == typeof(void))
        {
            // Optimize for 0-3 parameters and no return value
            if (p_FuncType.Parameters.Length == 0)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    p_Delegate.DynamicInvoke();
                });
            }

            if (p_FuncType.Parameters.Length == 1)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    p_Delegate.DynamicInvoke(l_Value.GetRawValue());
                });
            }

            if (p_FuncType.Parameters.Length == 2)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                    p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue());
                });
            }

            if (p_FuncType.Parameters.Length == 3)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value3 = p_Context.PopFromStack();
                    p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue(), l_Value3.GetRawValue());
                });
            }

            // Dynamic Fallbacks
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                Object[] l_Args = new Object[p_FuncType.Parameters.Length];
                for (int i = 0; i < l_Args.Length; i++)
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    l_Args[i] = l_Value.GetRawValue();
                }

                p_Delegate.DynamicInvoke(l_Args);
            });
        }


        if (p_FuncType.Results.Length == 1)
        {
            if (WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]) != p_Delegate.Method.ReturnType)
                throw new Exception("Import return type mismatch: " + p_FuncType.Results[0] + " != " + p_Delegate.Method.ReturnType);
        }


        // Optimize for 0-3 parameters and 1 return value
        if (p_FuncType.Parameters.Length == 0)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                Object l_Result = p_Delegate.DynamicInvoke();
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_Result));
            });
        }

        if (p_FuncType.Parameters.Length == 1)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                Object l_Result = p_Delegate.DynamicInvoke(l_Value.GetRawValue());
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_Result));
            });
        }

        if (p_FuncType.Parameters.Length == 2)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                Object l_Result = p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue());
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_Result));
            });
        }

        if (p_FuncType.Parameters.Length == 3)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                WebAssemblyInterpreterValue l_Value3 = p_Context.PopFromStack();
                Object l_Result = p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue(), l_Value3.GetRawValue());
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_Result));
            });
        }

        // Dnyamic Fallbacks
        return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
        {
            Object[] l_Args = new Object[p_FuncType.Parameters.Length];
            for (int i = 0; i < l_Args.Length; i++)
            {
                WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                l_Args[i] = l_Value.GetRawValue();
            }

            Object l_Result = p_Delegate.DynamicInvoke(l_Args);
            p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_Result));
        });
    }

    private Delegate CompileWasmImport(WasmFuncType p_FuncType, Delegate p_Delegate, IWebAssemblyMethod p_InternalMethod)
    {
        WasmFuncType l_ImportMetaData = p_InternalMethod.GetMetaData();

        if (!l_ImportMetaData.IsSame(p_FuncType))
            throw new Exception("Import method signature mismatch: " + l_ImportMetaData.ToString() + " != " + p_FuncType.ToString());

        // Void Result
        if (l_ImportMetaData.Results.Length == 0)
        {
            // Optimize for 0 parameters and Task<void> return value
            if (l_ImportMetaData.Parameters.Length == 0)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
                {
                    Task<object> l_Task = p_InternalMethod.Invoke();
                    await l_Task;    
                });
            }
            
            // Optimize for 1 parameter and Task<void> return value
            if (l_ImportMetaData.Parameters.Length == 1)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    Task<object> l_Task = p_InternalMethod.Invoke(l_Value.GetRawValue());
                    await l_Task;
                });
            }
            
            // Optimize for 2 parameters and Task<void> return value
            if (l_ImportMetaData.Parameters.Length == 2)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                    Task<object> l_Task = p_InternalMethod.Invoke(l_Value1.GetRawValue(), l_Value2.GetRawValue());
                    await l_Task;
                });
            }
            
            // Fallback for 3 and more parameters and Task<void> return value
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                Object[] l_Args = new Object[p_FuncType.Parameters.Length];
                for (int i = 0; i < l_Args.Length; i++)
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    l_Args[i] = l_Value.GetRawValue();
                }
                    
                Task<object> l_Task = p_InternalMethod.Invoke(l_Args);
                await l_Task;
            });
            
        }

        if (l_ImportMetaData.Results.Length == 1)
        {
        
            // Optimize for 0 parameters and Task<T> return value
            if (l_ImportMetaData.Parameters.Length == 0)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
                {
                    Task<object> l_Task = p_InternalMethod.Invoke();
                    object l_TaskValue = await l_Task;
                    p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
                });
            }
            
            // Optimize for 1 parameter and Task<T> return value
            if (l_ImportMetaData.Parameters.Length == 1)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    Task<object> l_Task = p_InternalMethod.Invoke(l_Value.GetRawValue());
                    object l_TaskValue = await l_Task;
                    p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
                });
            }
            
            // Optimize for 2 parameters and Task<T> return value
            if (l_ImportMetaData.Parameters.Length == 2)
            {
                return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
                {
                    WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                    Task<object> l_Task = p_InternalMethod.Invoke(l_Value1.GetRawValue(), l_Value2.GetRawValue());
                    object l_TaskValue = await l_Task;
                    p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
                });
            }
            
            // Fallback for 3 and more parameters and Task<T> return value
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                Object[] l_Args = new Object[p_FuncType.Parameters.Length];
                for (int i = 0; i < l_Args.Length; i++)
                {
                    WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                    l_Args[i] = l_Value.GetRawValue();
                }
                    
                Task<object> l_Task = p_InternalMethod.Invoke(l_Args);
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
            });
            
        }
        
        // Dynamic Fallbacks which can handle everything
        return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
        {
            Object[] l_Args = new Object[p_FuncType.Parameters.Length];
            for (int i = 0; i < l_Args.Length; i++)
            {
                WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                l_Args[i] = l_Value.GetRawValue();
            }

            Task<object> l_Task = p_InternalMethod.Invoke(l_Args);
            object l_TaskValue = await l_Task;

            // Void Result
            if (l_ImportMetaData.Results.Length == 0)
                return;

            // Single Result
            if (l_ImportMetaData.Results.Length == 1)
            {
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
                return;
            }

            // Multiple Results
            if (l_ImportMetaData.Results.Length > 1)
            {
                object[] l_Results = (object[])l_TaskValue;

                if (l_Results.Length != l_ImportMetaData.Results.Length)
                    throw new Exception("Import method result count mismatch: " + l_ImportMetaData.Results.Length + " != " + l_Results.Length);

                for (int i = 0; i < l_Results.Length; i++)
                {
                    p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[i], l_Results[i]));
                }

                return;
            }
        });
    }

    private Delegate CompileImportWithResultTask<T>(WasmFuncType p_FuncType, Delegate p_Delegate)
    {
        // Optimize for 0-3 parameters and Task<T> return value
        if (p_FuncType.Parameters.Length == 0)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke();
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
            });
        }

        if (p_FuncType.Parameters.Length == 1)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Value.GetRawValue());
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
            });
        }

        if (p_FuncType.Parameters.Length == 2)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue());
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
            });
        }

        if (p_FuncType.Parameters.Length == 3)
        {
            return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                WebAssemblyInterpreterValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyInterpreterValue l_Value2 = p_Context.PopFromStack();
                WebAssemblyInterpreterValue l_Value3 = p_Context.PopFromStack();
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Value1.GetRawValue(), l_Value2.GetRawValue(), l_Value3.GetRawValue());
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
            });
        }

        // Dnyamic Fallbacks
        return new WebAssemblyInterpreterVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
        {
            Object[] l_Args = new Object[p_FuncType.Parameters.Length];
            for (int i = 0; i < l_Args.Length; i++)
            {
                WebAssemblyInterpreterValue l_Value = p_Context.PopFromStack();
                l_Args[i] = l_Value.GetRawValue();
            }

            Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Args);
            object l_TaskValue = await l_Task;
            p_Context.PushToStack(WebAssemblyInterpreterValue.CreateDynamic(p_FuncType.Results[0], l_TaskValue));
        });
    }

    private T FindImportByName<T>(string p_Name) where T : WasmImport
    {
        foreach (WasmImport l_Import in m_WasmMetaData.Import)
        {
            if (!(l_Import is T))
                continue;

            if (l_Import.Name.Value == p_Name)
            {
                return (T)l_Import;
            }
        }

        return null;
    }

    private IWebAssemblyMethod GetOrCompileMethod(string p_Name)
    {
        if (m_ExportMethods.TryGetValue(p_Name, out IWebAssemblyMethod l_Method))
            return l_Method;

        IWebAssemblyMethod l_NewMethod = CompileMethod(p_Name);

        if (m_ExportMethods.TryAdd(p_Name, l_NewMethod))
            return l_NewMethod;

        return m_ExportMethods[p_Name];
    }

    private IWebAssemblyMethod CompileMethod(string p_Name)
    {
        int? l_Index = FindExportIndex(p_Name, WasmExternalKind.Function);

        if (l_Index == null)
            return new WebAssemblyInterpreterMethodNotFound(p_Name);

        long l_FinalIndex = m_WasmMetaData.FuncIndex[l_Index.Value];
        WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_FinalIndex];
        WasmCode l_Code = m_WasmMetaData.Code[l_Index.Value];

        return new WebAssemblyInterpreterMethod(m_VirtualMaschine, l_FuncType, l_Code);
    }

    private int? FindExportIndex(string p_Name, WasmExternalKind p_ExternalKind)
    {
        for (int i = 0; i < m_WasmMetaData.Export.Length; i++)
        {
            WasmExport l_WasmExport = m_WasmMetaData.Export[i];

            if (l_WasmExport.Name.Value == p_Name && l_WasmExport.Kind == p_ExternalKind)
            {
                // To Get the correct index we need to subtract the number of imports
                if (m_WasmMetaData.Import != null)
                    return (int)l_WasmExport.Index - m_WasmMetaData.Import.Count(x => x.Kind == p_ExternalKind);

                return (int)l_WasmExport.Index;
            }
        }

        return null;
    }

    public bool OptimizeInstruction(WasmInstruction p_Instruction)
    {
        if (p_Instruction is WasmCall)
        {
            WasmCall l_Call = (WasmCall)p_Instruction;
            WasmImportFunction l_WasmImport = (WasmImportFunction)m_WasmMetaData.Import[l_Call.FunctionIndex];

            WebAssemblyInterpreterImportMethod l_ImportMethod;

            if (!m_ImportMethods.TryGetValue(l_WasmImport, out l_ImportMethod))
            {
                WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_WasmImport.FunctionIndex];
                throw new Exception("Import method not found: " + l_WasmImport.Name.Value + l_FuncType.ToString());
            }


            l_Call.VmData = l_ImportMethod.Delegate;
            return true;
        }

        return false;
    }
}