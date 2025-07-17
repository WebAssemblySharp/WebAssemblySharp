using System;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Runtime.Utils;

public static class WebAssemblyImportUtils
{
    public static T FindImportByName<T>(WasmMetaData p_MetaData, string p_Name) where T : WasmImport
    {
        foreach (WasmImport l_Import in p_MetaData.Import)
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
    
    public static T FindImportByFilter<T>(WasmMetaData p_MetaData, Func<T, bool> p_Filter) where T : WasmImport
    {
        foreach (WasmImport l_Import in p_MetaData.Import)
        {
            if (!(l_Import is T))
                continue;

            if (p_Filter((T)l_Import))
            {
                return (T)l_Import;
            }
        }

        return null;
    }

    public static WasmFuncType GetFuncType(WasmMetaData p_MetaData, WasmImportFunction p_Function)
    {
        return p_MetaData.FunctionType[p_Function.FunctionIndex];
    }
    
}