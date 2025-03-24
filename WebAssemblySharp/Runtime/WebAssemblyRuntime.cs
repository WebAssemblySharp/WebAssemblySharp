using System;
using System.IO;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Readers.Binary;
using WebAssemblySharp.Runtime.JIT;

namespace WebAssemblySharp.Runtime;

public class WebAssemblyRuntime
{
    
    public async Task<WebAssemblyModule> LoadModule(Stream p_Stream)
    {
        
        WasmBinaryReader l_Reader = new WasmBinaryReader();
        
        byte[] l_Bytes = new Byte[255];
        
        while (true)
        {
            int l_ReadBlock = await p_Stream.ReadAsync(l_Bytes);
            
            if (l_ReadBlock != 0)
            {
                l_Reader.Read(l_Bytes.AsSpan().Slice(0, l_ReadBlock));
            }
            
            if (l_ReadBlock < l_Bytes.Length)
            {
                break;
            }
            
        }

        WasmMetaData l_WasmMetaData = l_Reader.Finish();
        
        // Load the code
        WebAssemblyModule l_Module = new WebAssemblyModule(new WebAssemblyJitExecutor(l_WasmMetaData));
        
        return l_Module;
        
    }
    
    
}