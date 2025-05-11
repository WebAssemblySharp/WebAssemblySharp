using System.Linq;

namespace WebAssemblySharp.MetaData.Utils;

public static class WasmMetaDataUtils
{
    
    public static int? FindExportIndex(WasmMetaData p_WasmMetaData, string p_Name, WasmExternalKind p_ExternalKind)
    {
        for (int i = 0; i < p_WasmMetaData.Export.Length; i++)
        {
            WasmExport l_WasmExport = p_WasmMetaData.Export[i];

            if (l_WasmExport.Name.Value == p_Name && l_WasmExport.Kind == p_ExternalKind)
            {
                // To Get the correct index we need to subtract the number of imports
                if (p_WasmMetaData.Import != null)
                    return (int)l_WasmExport.Index - p_WasmMetaData.Import.Count(x => x.Kind == p_ExternalKind);

                return (int)l_WasmExport.Index;
            }
        }

        return null;
    }
    
}