using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Instructions;
using WebAssemblySharp.Readers.Binary;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitExecutor : IWebAssemblyExecutor, IWebAssemblyJitInteropOptimizer
{
    private WasmMetaData m_WasmMetaData;
    private ConcurrentDictionary<String, IWebAssemblyMethod> m_ExportMethods;
    private ConcurrentDictionary<WasmImport, WebAssemblyJitImportMethod> m_ImportMethods;
    private WebAssemblyJitVirtualMaschine m_VirtualMaschine;

    public WebAssemblyJitExecutor()
    {
        m_ExportMethods = new ConcurrentDictionary<String, IWebAssemblyMethod>();
        m_ImportMethods = new ConcurrentDictionary<WasmImport, WebAssemblyJitImportMethod>();
        m_VirtualMaschine = new WebAssemblyJitVirtualMaschine();
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
        m_VirtualMaschine.OptimizeCode(this, m_WasmMetaData);
    }

    public void DefineImport(string p_Name, Delegate p_Delegate)
    {
        WasmImport l_Import = FindImportByName(p_Name);

        if (l_Import == null)
            throw new Exception("Import not found: " + p_Name);

        WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_Import.Index];

        Delegate l_Delegate = CompileImport(l_FuncType, p_Delegate);
        WebAssemblyJitImportMethod l_ImportMethod = new WebAssemblyJitImportMethod(l_Import, l_FuncType, l_Delegate);

        if (!m_ImportMethods.TryAdd(l_Import, l_ImportMethod))
            throw new Exception("Import already defined: " + p_Name);
    }

    private Delegate CompileImport(WasmFuncType p_FuncType, Delegate p_Delegate)
    {
        
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
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    return (Task)p_Delegate.DynamicInvoke();
                });
            }

            if (p_FuncType.Parameters.Length == 1)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                    return (Task)p_Delegate.DynamicInvoke(l_Value.Value);
                });
            }

            if (p_FuncType.Parameters.Length == 2)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                    return (Task)p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value);
                });
            }

            if (p_FuncType.Parameters.Length == 3)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
                {
                    WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                    WebAssemblyJitValue l_Value3 = p_Context.PopFromStack();
                    return (Task)p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value, l_Value3.Value);
                });
            }

            // Dynamic Fallbacks
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync((p_Instruction, p_Context) =>
            {
                Object[] l_Args = new Object[p_FuncType.Parameters.Length];
                for (int i = 0; i < l_Args.Length; i++)
                {
                    WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                    l_Args[i] = l_Value.Value;
                }

                return (Task)p_Delegate.DynamicInvoke(l_Args);
            });
        }

        if (p_Delegate.Method.ReturnType == typeof(void))
        {
            // Optimize for 0-3 parameters and no return value
            if (p_FuncType.Parameters.Length == 0)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) => { p_Delegate.DynamicInvoke(); });
            }

            if (p_FuncType.Parameters.Length == 1)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                    p_Delegate.DynamicInvoke(l_Value.Value);
                });
            }

            if (p_FuncType.Parameters.Length == 2)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                    p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value);
                });
            }

            if (p_FuncType.Parameters.Length == 3)
            {
                return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
                {
                    WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                    WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                    WebAssemblyJitValue l_Value3 = p_Context.PopFromStack();
                    p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value, l_Value3.Value);
                });
            }

            // Dynamic Fallbacks
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                Object[] l_Args = new Object[p_FuncType.Parameters.Length];
                for (int i = 0; i < l_Args.Length; i++)
                {
                    WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                    l_Args[i] = l_Value.Value;
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
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                Object l_Result = p_Delegate.DynamicInvoke();
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_Result));
            });
        }

        if (p_FuncType.Parameters.Length == 1)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                Object l_Result = p_Delegate.DynamicInvoke(l_Value.Value);
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_Result));
            });
        }

        if (p_FuncType.Parameters.Length == 2)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                Object l_Result = p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value);
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_Result));
            });
        }

        if (p_FuncType.Parameters.Length == 3)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
            {
                WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                WebAssemblyJitValue l_Value3 = p_Context.PopFromStack();
                Object l_Result = p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value, l_Value3.Value);
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_Result));
            });
        }

        // Dnyamic Fallbacks
        return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegate((p_Instruction, p_Context) =>
        {
            Object[] l_Args = new Object[p_FuncType.Parameters.Length];
            for (int i = 0; i < l_Args.Length; i++)
            {
                WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                l_Args[i] = l_Value.Value;
            }

            Object l_Result = p_Delegate.DynamicInvoke(l_Args);
            p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_Result));
        });
    }

    private Delegate CompileImportWithResultTask<T>(WasmFuncType p_FuncType, Delegate p_Delegate)
    {
        
        // Optimize for 0-3 parameters and Task<T> return value
        if (p_FuncType.Parameters.Length == 0)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke();
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_TaskValue));
            });
        }

        if (p_FuncType.Parameters.Length == 1)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Value.Value);
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_TaskValue));
            });
        }

        if (p_FuncType.Parameters.Length == 2)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value);
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_TaskValue));
            });
        }

        if (p_FuncType.Parameters.Length == 3)
        {
            return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
            {
                WebAssemblyJitValue l_Value1 = p_Context.PopFromStack();
                WebAssemblyJitValue l_Value2 = p_Context.PopFromStack();
                WebAssemblyJitValue l_Value3 = p_Context.PopFromStack();
                Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Value1.Value, l_Value2.Value, l_Value3.Value);
                object l_TaskValue = await l_Task;
                p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_TaskValue));
            });
        }

        // Dnyamic Fallbacks
        return new WebAssemblyJitVirtualMaschine.ExecuteInstructionDelegateAsync(async (p_Instruction, p_Context) =>
        {
            Object[] l_Args = new Object[p_FuncType.Parameters.Length];
            for (int i = 0; i < l_Args.Length; i++)
            {
                WebAssemblyJitValue l_Value = p_Context.PopFromStack();
                l_Args[i] = l_Value.Value;
            }

            Task<T> l_Task = (Task<T>)p_Delegate.DynamicInvoke(l_Args);
            object l_TaskValue = await l_Task;
            p_Context.PushToStack(new WebAssemblyJitValue(p_FuncType.Results[0], l_TaskValue));
        });
    }

    private WasmImport FindImportByName(string p_Name)
    {
        foreach (WasmImport l_Import in m_WasmMetaData.Import)
        {
            if (l_Import.Name.Value == p_Name)
            {
                return l_Import;
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
            return new WebAssemblyJitMethodNotFound(p_Name);

        WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_Index.Value];
        WasmCode l_Code = m_WasmMetaData.Code[l_Index.Value];

        return new WebAssemblyJitMethod(m_VirtualMaschine, l_FuncType, l_Code);
    }

    private int? FindExportIndex(string p_Name, WasmExternalKind p_ExternalKind)
    {
        for (int i = 0; i < m_WasmMetaData.Export.Length; i++)
        {
            WasmExport l_WasmExport = m_WasmMetaData.Export[i];

            if (l_WasmExport.Name.Value == p_Name && l_WasmExport.Kind == p_ExternalKind)
            {
                return i;
            }
        }

        return null;
    }

    public bool OptimizeInstruction(WasmInstruction p_Instruction)
    {
        if (p_Instruction is WasmCall)
        {
            WasmCall l_Call = (WasmCall)p_Instruction;
            WasmImport l_WasmImport = m_WasmMetaData.Import[l_Call.FunctionIndex];

            WebAssemblyJitImportMethod l_ImportMethod;

            if (!m_ImportMethods.TryGetValue(l_WasmImport, out l_ImportMethod))
                throw new Exception("Import method not found: " + l_WasmImport.Name.Value);

            l_Call.VmData = l_ImportMethod.Delegate;
            return true;
        }

        return false;
    }
}