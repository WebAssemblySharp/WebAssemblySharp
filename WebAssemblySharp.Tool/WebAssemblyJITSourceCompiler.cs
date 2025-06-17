using System;
using System.IO;
using WebAssemblySharp.MetaData;

namespace WebAssemblySharp.Tool;

public class WebAssemblyJITSourceCompiler: WebAssemblyJITCompiler
{
    private readonly string m_AssemblyName;

    public WebAssemblyJITSourceCompiler(String p_AssemblyName, WasmMetaData p_WasmMetaData) : base(p_WasmMetaData, null)
    {
        m_AssemblyName = p_AssemblyName;
    }

    public MemoryStream CreateAssemblyStream()
    {
        Type l_Type = m_TypeBuilder.CreateType();
        var l_Generator = new Lokad.ILPack.AssemblyGenerator();
        byte[] l_GenerateAssemblyBytes = l_Generator.GenerateAssemblyBytes(l_Type.Assembly);
        return new MemoryStream(l_GenerateAssemblyBytes);
    }

    protected override string GetDynamicTypeName()
    {
        return m_AssemblyName;
    }
}