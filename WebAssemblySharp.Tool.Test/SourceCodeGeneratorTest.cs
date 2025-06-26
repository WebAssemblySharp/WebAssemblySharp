using System.IO;
using System.Threading.Tasks;
using WebAssemblySharpExampleData;

namespace WebAssemblySharp.Tool.Test;

[TestClass]
public class SourceCodeGeneratorTest
{
    
    [TestMethod]
    public async Task GenerateSourceCodeTest()
    {
        DirectoryInfo l_TempDir = Directory.CreateTempSubdirectory();
        DirectoryInfo l_Input = l_TempDir.CreateSubdirectory("Input");
        
        Stream l_Stream = typeof(WebAssemblyExamples).Assembly.GetManifestResourceStream("WebAssemblySharpExampleData.Programms.isprime.wasm");
        string l_WasmFile = Path.Combine(l_Input.FullName, "IsPrime.wasm");
        
        using (FileStream l_FileStream = new FileStream(l_WasmFile, FileMode.Create, FileAccess.Write))
        {
            await l_Stream.CopyToAsync(l_FileStream);
            await l_FileStream.FlushAsync();
        }
        
        DirectoryInfo l_Output = l_TempDir.CreateSubdirectory("Output");
            
        await SourceCodeGenerator.Generate(l_WasmFile, l_Output.FullName);

        l_Output.Refresh();
        Assert.IsTrue(l_Output.Exists, "Output directory does not exist.");
        Assert.IsTrue(l_Output.GetFiles().Length > 0, "No C# files generated in output directory.");
        Assert.AreEqual("IsPrimeModule.cs", l_Output.GetFiles()[0].Name);
        
        
        Directory.Delete(l_TempDir.FullName, true);
        
    }
    
}