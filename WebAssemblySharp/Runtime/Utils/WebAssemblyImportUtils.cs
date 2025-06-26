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
    
}