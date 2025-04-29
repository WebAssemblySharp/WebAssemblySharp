using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Readers.Binary;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Readers;

[TestClass]
public class WasmBinaryReaderTest
{
    [DataTestMethod]
    [DataRow("WebAssemblySharpExampleData.Programms.add.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.isprime.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.itoa.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.loops.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.imports.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.ifexpr.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.locals.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.memory-basics.wasm")]
    public async Task ReadTest(string p_SourcePath)
    {
        using (Stream l_Stream = typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream(p_SourcePath))
        {
            WasmBinaryReader l_WasmReader = new WasmBinaryReader();

            byte[] l_Bytes = new Byte[255];

            while (true)
            {
                int l_ReadBlock = await l_Stream.ReadAsync(l_Bytes);

                if (l_ReadBlock != 0)
                {
                    l_WasmReader.Read(l_Bytes.AsSpan().Slice(0, l_ReadBlock));
                }
                    
                if (l_ReadBlock < l_Bytes.Length)
                {
                    break;
                }
                    
            }

            Finish(l_WasmReader);
        }
    }
    
    
    [DataTestMethod]
    [DataRow("WebAssemblySharpExampleData.Programms.add.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.isprime.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.itoa.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.loops.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.imports.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.ifexpr.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.locals.wasm")]
    [DataRow("WebAssemblySharpExampleData.Programms.memory-basics.wasm")]
    public async Task ReadTestSlow(string p_SourcePath)
    {
        using (Stream l_Stream = typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream(p_SourcePath))
        {
            WasmBinaryReader l_WasmReader = new WasmBinaryReader();

            byte[] l_Bytes = new Byte[1];

            while (true)
            {
                int l_ReadBlock = await l_Stream.ReadAsync(l_Bytes);

                if (l_ReadBlock != 0)
                {
                    l_WasmReader.Read(l_Bytes.AsSpan().Slice(0, l_ReadBlock));
                }
                    
                if (l_ReadBlock < l_Bytes.Length)
                {
                    break;
                }
                    
            }

            Finish(l_WasmReader);
        }
    }
    

    private static void Finish(WasmBinaryReader l_WasmReader)
    {
        WasmMetaData l_WasmMetaData = l_WasmReader.Finish();
        Assert.IsNotNull(l_WasmMetaData);


        /*
            List<WasmReaderElement> l_Elements = l_WasmReader.Finish();

            string l_Invalid = string.Join(Environment.NewLine, l_Elements.Where(IsInvalid));
            Assert.AreEqual("", l_Invalid);

            List<string> l_ElementsText = l_Elements.Select(p_Element => p_Element.ToText()).ToList();

            string l_ElementsString = string.Join(" ", l_ElementsText);
            string l_ExpectedResult = await ReadResourceFile(p_ExpectedResultPath);

            Assert.AreEqual(l_ExpectedResult, l_ElementsString);
            */
    }

    private async Task<string> ReadResourceFile(String p_ExpectedResultPath)
    {
        using (Stream l_Stream = typeof(WasmTextReaderTest).Assembly.GetManifestResourceStream(p_ExpectedResultPath))
        {
            using (TextReader l_Reader = new StreamReader(l_Stream, Encoding.UTF8))
            {
                return await l_Reader.ReadToEndAsync();
            }
        }
    }
}