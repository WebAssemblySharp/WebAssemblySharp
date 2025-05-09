﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAssemblySharp.Readers.Text;
using WebAssemblySharp.Readers.Text.Model;
using WebAssemblySharpExampleData;

namespace WebAssemblySharpTest.Readers;

[TestClass]
public class WasmTextReaderTest
{
    [DataTestMethod]
    [DataRow("WebAssemblySharpExampleData.Programms.add.wat", "WebAssemblySharpTest.Data.Result.addReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.isprime.wat", "WebAssemblySharpTest.Data.Result.isprimeReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.itoa.wat", "WebAssemblySharpTest.Data.Result.itoaReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.loops.wat", "WebAssemblySharpTest.Data.Result.loopsReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.imports.wat", "WebAssemblySharpTest.Data.Result.importsReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.ifexpr.wat", "WebAssemblySharpTest.Data.Result.ifexprReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.locals.wat", "WebAssemblySharpTest.Data.Result.localsReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.memory-basics.wat", "WebAssemblySharpTest.Data.Result.memory-basicsReaderResult.txt")]
    public async Task ReadTest(string p_SourcePath, string p_ExpectedResultPath)
    {
        using (Stream l_Stream = typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream(p_SourcePath))
        {
            WasmTextReader l_WasmTextReader = new WasmTextReader();

            using (TextReader l_Reader = new StreamReader(l_Stream, Encoding.UTF8))
            {
                char[] l_Buffer = new char[255];

                while (true)
                {
                    int l_ReadBlock = await l_Reader.ReadBlockAsync(l_Buffer);

                    if (l_ReadBlock != 0)
                    {
                        l_WasmTextReader.Read(l_Buffer.AsSpan().Slice(0, l_ReadBlock));
                    }
                    
                    if (l_ReadBlock < l_Buffer.Length)
                    {
                        break;
                    }
                    
                }
            }
            
            await Finish(p_ExpectedResultPath, l_WasmTextReader);
        }
    }
    
    
    [DataTestMethod]
    [DataRow("WebAssemblySharpExampleData.Programms.add.wat", "WebAssemblySharpTest.Data.Result.addReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.isprime.wat", "WebAssemblySharpTest.Data.Result.isprimeReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.itoa.wat", "WebAssemblySharpTest.Data.Result.itoaReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.loops.wat", "WebAssemblySharpTest.Data.Result.loopsReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.imports.wat", "WebAssemblySharpTest.Data.Result.importsReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.ifexpr.wat", "WebAssemblySharpTest.Data.Result.ifexprReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.locals.wat", "WebAssemblySharpTest.Data.Result.localsReaderResult.txt")]
    [DataRow("WebAssemblySharpExampleData.Programms.memory-basics.wat", "WebAssemblySharpTest.Data.Result.memory-basicsReaderResult.txt")]
    public async Task ReadTestSlow(string p_SourcePath, string p_ExpectedResultPath)
    {
        using (Stream l_Stream = typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream(p_SourcePath))
        {
            WasmTextReader l_WasmTextReader = new WasmTextReader();

            using (TextReader l_Reader = new StreamReader(l_Stream, Encoding.UTF8))
            {
                char[] l_Buffer = new char[1];

                while (true)
                {
                    int l_ReadBlock = await l_Reader.ReadBlockAsync(l_Buffer);

                    if (l_ReadBlock != 0)
                    {
                        l_WasmTextReader.Read(l_Buffer.AsSpan().Slice(0, l_ReadBlock));
                    }
                    
                    if (l_ReadBlock < l_Buffer.Length)
                    {
                        break;
                    }
                    
                }
            }
            
            await Finish(p_ExpectedResultPath, l_WasmTextReader);
        }
    }
    

    private async Task Finish(string p_ExpectedResultPath, WasmTextReader l_WasmTextReader)
    {
        List<WasmReaderElement> l_Elements = l_WasmTextReader.Finish();

        string l_Invalid = string.Join(Environment.NewLine, l_Elements.Where(IsInvalid));
        Assert.AreEqual("", l_Invalid);
            
        List<string> l_ElementsText = l_Elements.Select(p_Element => p_Element.ToText()).ToList();

        string l_ElementsString = string.Join(" ", l_ElementsText);
        string l_ExpectedResult = await ReadResourceFile(p_ExpectedResultPath);

        Assert.AreEqual(l_ExpectedResult, l_ElementsString);
    }

    private bool IsInvalid(WasmReaderElement p_Element)
    {
        if (p_Element.Kind == WasmReaderElementKind.Text)
        {
            if (string.IsNullOrWhiteSpace(p_Element.Data))
                return true;

            if (p_Element.Data[0] != '"')
            {
                if (p_Element.Data.Contains("."))
                {
                    return true;
                }
            }
            else
            {
                if (p_Element.Data[p_Element.Data.Length - 1] != '"')
                {
                    return true;
                }
            }
        }

        return false;
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