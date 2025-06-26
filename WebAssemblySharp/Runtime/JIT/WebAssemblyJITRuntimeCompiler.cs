using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Utils;
using WebAssemblySharp.Runtime.Utils;

namespace WebAssemblySharp.Runtime.JIT;

[RequiresDynamicCode("WebAssemblyJITRuntimeCompiler requires dynamic code.")]
public class WebAssemblyJITRuntimeCompiler: WebAssemblyJITCompiler
{
    public WebAssemblyJITRuntimeCompiler(WasmMetaData p_WasmMetaData, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type p_ProxyType) : base(p_WasmMetaData, p_ProxyType)
    {
    }

    public WebAssemblyJITAssembly BuildAssembly(IDictionary<int, IWebAssemblyMemoryArea> p_ImportedMemoryAreas,
        IDictionary<WasmImportFunction, Delegate> p_ImportMethods)
    {
        Type l_Type = m_TypeBuilder.CreateType();
        object l_Instance = Activator.CreateInstance(l_Type);
        
        IDictionary l_ExportedMethods = new HybridDictionary();
        
        foreach (KeyValuePair<string, MethodInfo> l_Pair in m_ExportedMethods)
        {
            // Find the function index in the module
            int? l_FunctionIndex = WasmMetaDataUtils.FindExportIndex(m_WasmMetaData, l_Pair.Key, WasmExternalKind.Function);

            if (!l_FunctionIndex.HasValue)
                throw new Exception($"MetaData for Export not found: {l_Pair.Key}");

            long l_FinalIndex = m_WasmMetaData.FuncIndex[l_FunctionIndex.Value];
            WasmFuncType l_FuncType = m_WasmMetaData.FunctionType[l_FinalIndex];

            MethodInfo l_MethodInfo = l_Type.GetMethod(l_Pair.Key);

            IWebAssemblyMethod l_WebAssemblyMethod = CreateMethod(l_Instance, l_MethodInfo, l_FuncType);
            l_ExportedMethods.Add(l_Pair.Key, l_WebAssemblyMethod);
        }
        
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
                l_RuntimeFieldInfo.SetValue(l_Instance, l_Memory);
            }
        }
        
        IWebAssemblyMemoryArea[] l_MemoryAreas = new IWebAssemblyMemoryArea[m_MemoryFields.Count];
        
        for (int i = 0; i < m_MemoryFields.Count; i++)
        {
            FieldInfo l_RuntimeFieldInfo = l_Type.GetField(m_MemoryFields[i].Name);

            // Constant expression representing the specific instance of MyClass
            ConstantExpression l_InstanceExpression = Expression.Constant(l_Instance, l_Type);
            // Field expression representing accessing the field
            MemberExpression l_FieldAccess = Expression.Field(l_InstanceExpression, l_RuntimeFieldInfo);
            
            // Getter
            // Lambda expression representing the lambda that returns the field value
            Expression<Func<byte[]>> l_Lambda = Expression.Lambda<Func<byte[]>>(l_FieldAccess);
            Func<byte[]> l_MemoryAccess = l_Lambda.Compile();
            
            // Setter
            ParameterExpression l_Parameter = Expression.Parameter(typeof(byte[]));
            BinaryExpression l_Assign = Expression.Assign(l_FieldAccess, l_Parameter);
            Action<byte[]> l_MemoryUpdate = Expression.Lambda<Action<byte[]>>(l_Assign, l_Parameter).Compile();
            
            l_MemoryAreas[i] = new WebAssemblyJITMemoryArea(l_MemoryAccess, l_MemoryUpdate, (int)m_WasmMetaData.Memory[i].Max);
            
        }
        
        Reset();
        
        return new WebAssemblyJITAssembly(l_ExportedMethods, l_Instance, l_MemoryAreas);
    }

    [SuppressMessage("Warning", "IL2076")]
    private IWebAssemblyMethod CreateMethod(object p_Instance, MethodInfo p_MethodInfo, WasmFuncType p_FuncType)
    {
        List<Type> l_ParameterTypes = p_FuncType.Parameters.Select(x => WebAssemblyDataTypeUtils.GetInternalType(x)).ToList();

        if (p_FuncType.Results.Length == 0)
        {
            return CreateVoidMethod(l_ParameterTypes, p_Instance, p_MethodInfo, p_FuncType);
        }
        else if (p_FuncType.Results.Length == 1)
        {
            l_ParameterTypes.Add(WebAssemblyDataTypeUtils.GetInternalType(p_FuncType.Results[0]));
        }
        else
        {
            l_ParameterTypes.Add(WebAssemblyValueTupleUtils.GetValueTupleType(p_FuncType.Results));
        }

        Type l_DelegateType;

        if (l_ParameterTypes.Count == 0)
        {
            throw new Exception("No Gerneric parameters found");
        }
        else if (l_ParameterTypes.Count == 1)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<>).MakeGenericType(l_ParameterTypes[0]);
        }
        else if (l_ParameterTypes.Count == 2)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1]);
        }
        else if (l_ParameterTypes.Count == 3)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2]);
        }
        else if (l_ParameterTypes.Count == 4)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3]);
        }
        else if (l_ParameterTypes.Count == 5)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4]);
        }
        else if (l_ParameterTypes.Count == 6)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4], l_ParameterTypes[5]);
        }
        else if (l_ParameterTypes.Count == 7)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6]);
        }
        else if (l_ParameterTypes.Count == 8)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2], l_ParameterTypes[3],
                l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7]);
        }
        else if (l_ParameterTypes.Count == 9)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8]);
        }
        else if (l_ParameterTypes.Count == 10)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9]);
        }
        else if (l_ParameterTypes.Count == 11)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10]);
        }
        else if (l_ParameterTypes.Count == 12)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11]);
        }
        else if (l_ParameterTypes.Count == 13)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12]);
        }
        else if (l_ParameterTypes.Count == 14)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13]);
        }
        else if (l_ParameterTypes.Count == 15)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14]);
        }
        else if (l_ParameterTypes.Count == 16)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14],
                l_ParameterTypes[15]);
        }
        else if (l_ParameterTypes.Count == 17)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorMethod<,,,,,,,,,,,,,,,,>).MakeGenericType(l_ParameterTypes[0], l_ParameterTypes[1], l_ParameterTypes[2],
                l_ParameterTypes[3], l_ParameterTypes[4], l_ParameterTypes[5], l_ParameterTypes[6], l_ParameterTypes[7], l_ParameterTypes[8],
                l_ParameterTypes[9], l_ParameterTypes[10], l_ParameterTypes[11], l_ParameterTypes[12], l_ParameterTypes[13], l_ParameterTypes[14],
                l_ParameterTypes[15], l_ParameterTypes[16]);
        }
        else
        {
            throw new Exception($"Too many parameters: {l_ParameterTypes.Count}");
        }

        return (IWebAssemblyMethod)Activator.CreateInstance(l_DelegateType, p_Instance, p_FuncType, p_MethodInfo);
    }
    
    [SuppressMessage("Warning", "IL2076")]
    private IWebAssemblyMethod CreateVoidMethod(List<Type> p_ParameterTypes, object p_Instance, MethodInfo p_MethodInfo, WasmFuncType p_FuncType)
    {
        
        Type l_DelegateType;

        if (p_ParameterTypes.Count == 0)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod);
        }
        else if (p_ParameterTypes.Count == 1)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<>).MakeGenericType(p_ParameterTypes[0]);
        }
        else if (p_ParameterTypes.Count == 2)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1]);
        }
        else if (p_ParameterTypes.Count == 3)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2]);
        }
        else if (p_ParameterTypes.Count == 4)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3]);
        }
        else if (p_ParameterTypes.Count == 5)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4]);
        }
        else if (p_ParameterTypes.Count == 6)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4], p_ParameterTypes[5]);
        }
        else if (p_ParameterTypes.Count == 7)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6]);
        }
        else if (p_ParameterTypes.Count == 8)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2], p_ParameterTypes[3],
                p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7]);
        }
        else if (p_ParameterTypes.Count == 9)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8]);
        }
        else if (p_ParameterTypes.Count == 10)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9]);
        }
        else if (p_ParameterTypes.Count == 11)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10]);
        }
        else if (p_ParameterTypes.Count == 12)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11]);
        }
        else if (p_ParameterTypes.Count == 13)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12]);
        }
        else if (p_ParameterTypes.Count == 14)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12], p_ParameterTypes[13]);
        }
        else if (p_ParameterTypes.Count == 15)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12], p_ParameterTypes[13], p_ParameterTypes[14]);
        }
        else if (p_ParameterTypes.Count == 16)
        {
            l_DelegateType = typeof(WebAssemblyJITExecutorVoidMethod<,,,,,,,,,,,,,,,>).MakeGenericType(p_ParameterTypes[0], p_ParameterTypes[1], p_ParameterTypes[2],
                p_ParameterTypes[3], p_ParameterTypes[4], p_ParameterTypes[5], p_ParameterTypes[6], p_ParameterTypes[7], p_ParameterTypes[8],
                p_ParameterTypes[9], p_ParameterTypes[10], p_ParameterTypes[11], p_ParameterTypes[12], p_ParameterTypes[13], p_ParameterTypes[14],
                p_ParameterTypes[15]);
        }
        else
        {
            throw new Exception($"Too many parameters: {p_ParameterTypes.Count}");
        }

        return (IWebAssemblyMethod)Activator.CreateInstance(l_DelegateType, p_Instance, p_FuncType, p_MethodInfo);
    }
}