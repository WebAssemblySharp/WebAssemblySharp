using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitExecutor : IWebAssemblyExecutor
{
    private readonly WasmMetaData m_WasmMetaData;
    private Dictionary<String, IWebAssemblyMethod> m_Methods;
    private ReaderWriterLockSlim m_MethodsLock = new ReaderWriterLockSlim();
    private WebAssemblyJitVirtualMaschine m_VirtualMaschine;

    public WebAssemblyJitExecutor(WasmMetaData p_WasmMetaData)
    {
        m_WasmMetaData = p_WasmMetaData;
        m_Methods = new Dictionary<String, IWebAssemblyMethod>();
        m_VirtualMaschine = new WebAssemblyJitVirtualMaschine();
    }
    
    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        return GetOrCompileMethod(p_Name);  
    }

    private IWebAssemblyMethod GetOrCompileMethod(string p_Name)
    {
        m_MethodsLock.EnterReadLock();
        try
        {
            if (m_Methods.TryGetValue(p_Name, out IWebAssemblyMethod l_Method))
                return l_Method;
        }
        finally
        {
            m_MethodsLock.ExitReadLock();
        }
        
        IWebAssemblyMethod l_NewMethod = CompileMethod(p_Name);
        
        m_MethodsLock.EnterWriteLock();
        try
        {
            // Double check in case another thread added the method while we were compiling
            if (m_Methods.TryGetValue(p_Name, out IWebAssemblyMethod l_Method))
                return l_Method;
            
            m_Methods.Add(p_Name, l_NewMethod);
            return l_NewMethod;
        }
        finally
        {
            m_MethodsLock.ExitWriteLock();
        }
        
        
    }

    private IWebAssemblyMethod CompileMethod(string p_Name)
    {
        WasmExport l_Export = FindExport(p_Name, WasmExternalKind.Function);

        if (l_Export == null)
            return new WebAssemblyJitMethodNotFound(p_Name);

        WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_Export.Index];
        WasmCode l_Code = m_WasmMetaData.Code[m_WasmMetaData.FuncIndex[l_Export.Index]];
        
        return new WebAssemblyJitMethod(m_VirtualMaschine, l_FuncType, l_Code);
    }

    private WasmExport FindExport(string p_Name, WasmExternalKind p_ExternalKind)
    {
        foreach (var l_Export in m_WasmMetaData.Export)
        {
            if (l_Export.Name.Value == p_Name && l_Export.Kind == p_ExternalKind)
            {
                return l_Export;
            }
        }
        return null;
    }
}