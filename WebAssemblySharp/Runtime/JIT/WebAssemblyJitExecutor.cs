using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitExecutor : IWebAssemblyExecutor
{
    private WasmMetaData m_WasmMetaData;
    private ConcurrentDictionary<String, IWebAssemblyMethod> m_Methods;
    private WebAssemblyJitVirtualMaschine m_VirtualMaschine;

    public WebAssemblyJitExecutor()
    {
        m_Methods = new ConcurrentDictionary<String, IWebAssemblyMethod>();
        m_VirtualMaschine = new WebAssemblyJitVirtualMaschine();
    }
    
    public IWebAssemblyMethod GetMethod(string p_Name)
    {
        return GetOrCompileMethod(p_Name);  
    }

    public void LoadCode(WasmMetaData p_WasmMetaData)
    {
        m_WasmMetaData = p_WasmMetaData;
        m_VirtualMaschine.OptimizeCode(p_WasmMetaData);

    }

    private IWebAssemblyMethod GetOrCompileMethod(string p_Name)
    {
        if (m_Methods.TryGetValue(p_Name, out IWebAssemblyMethod l_Method))
            return l_Method;
        
        IWebAssemblyMethod l_NewMethod = CompileMethod(p_Name);
        
        if (m_Methods.TryAdd(p_Name, l_NewMethod))
            return l_NewMethod;

        return m_Methods[p_Name];
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