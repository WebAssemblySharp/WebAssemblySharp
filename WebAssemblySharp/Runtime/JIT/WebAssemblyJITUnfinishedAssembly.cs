using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJITUnfinishedAssembly: WebAssemblyJITAssembly
{
    private readonly List<FieldInfo> m_MemoryFields;
    private readonly List<FieldInfo> m_AsyncExternalFunctionFields;
    private readonly List<FieldInfo> m_SyncExternalFunctionFields;

    public WebAssemblyJITUnfinishedAssembly(IDictionary p_ExportedMethodes, object p_Instance, IWebAssemblyMemoryArea[] p_MemoryAreas, List<FieldInfo> p_MemoryFields, List<FieldInfo> p_AsyncExternalFunctionFields, List<FieldInfo> p_SyncExternalFunctionFields) : base(p_ExportedMethodes, p_Instance, p_MemoryAreas)
    {
        m_MemoryFields = p_MemoryFields;
        m_AsyncExternalFunctionFields = p_AsyncExternalFunctionFields;
        m_SyncExternalFunctionFields = p_SyncExternalFunctionFields;
    }

    
    public WebAssemblyJITAssembly Finalize(IDictionary<int, IWebAssemblyMemoryArea> p_ImportedMemoryAreas, IDictionary<int, Delegate> p_ImportMethods)
    {
        Type l_Type = Instance.GetType();
        
        if (p_ImportedMemoryAreas.Count > 0)
        {
            int l_Max = p_ImportedMemoryAreas.Keys.Max();
            
            for (int i = 0; i < l_Max + 1; i++)
            {
                byte[] l_Memory;
                
                if (p_ImportedMemoryAreas.TryGetValue(i, out IWebAssemblyMemoryArea l_MemoryArea))
                {
                    l_Memory = l_MemoryArea.GetInternalMemory();
                }
                else
                {
                    l_Memory = new byte[0];
                }
                
                FieldInfo l_RuntimeFieldInfo = l_Type.GetField(m_MemoryFields[i].Name);
                l_RuntimeFieldInfo.SetValue(Instance, l_Memory);
            }
        }
        
        // Connect to imported Methods
        foreach (KeyValuePair<int, Delegate> l_ImportMethod in p_ImportMethods)
        {
            FieldInfo l_RuntimeFieldInfo;
            
            if (WebAssemblyJITCompilerUtils.IsAsyncFuncResultType(l_ImportMethod.Value.Method.ReturnType))
            {
                l_RuntimeFieldInfo = l_Type.GetField(m_AsyncExternalFunctionFields[l_ImportMethod.Key].Name);        
            }
            else
            {
                l_RuntimeFieldInfo = l_Type.GetField(m_SyncExternalFunctionFields[l_ImportMethod.Key].Name);     
            }
            
            l_RuntimeFieldInfo.SetValue(Instance, l_ImportMethod.Value);
                
        }
        
        return new WebAssemblyJITAssembly(ExportedMethodes, Instance, MemoryAreas);
    } 
}