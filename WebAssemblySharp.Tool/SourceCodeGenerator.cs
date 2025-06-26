using System;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.Readers.Binary;

namespace WebAssemblySharp.Tool;

/// <summary>
/// SourceCodeGenerator is responsible for reading a WebAssembly (.wasm) file,
/// extracting its metadata, compiling it to an in-memory .NET assembly,
/// and decompiling the resulting assembly into C# source code files.
/// The generated source files are written to the specified output directory.
/// </summary>
public class SourceCodeGenerator
{
    public static async Task Generate(String p_PathToWasmFile, String p_OutputDir)
    {
        Console.WriteLine($"Reading '{p_PathToWasmFile}'...");

        WasmMetaData l_WasmMetaData;

        using (FileStream l_Input = new FileStream(p_PathToWasmFile, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            WasmBinaryReader l_Reader = new WasmBinaryReader();

            byte[] l_Bytes = new Byte[255];

            while (true)
            {
                int l_ReadBlock = await l_Input.ReadAsync(l_Bytes);

                if (l_ReadBlock != 0)
                {
                    l_Reader.Read(l_Bytes.AsSpan().Slice(0, l_ReadBlock));
                }

                if (l_ReadBlock < l_Bytes.Length)
                {
                    break;
                }
            }

            l_WasmMetaData = l_Reader.Finish();
        }

        Console.WriteLine($"Wasm file '{p_PathToWasmFile}' read successfully.");

        Console.WriteLine($"Generating IL to '{p_OutputDir}'...");

        string l_WasmAssemblyName = Path.GetFileNameWithoutExtension(p_PathToWasmFile).Replace(".", "_").Replace("-", "_").Replace(" ", "_") + "Module";

        WebAssemblyInMemoryCompiler l_Compiler = new WebAssemblyInMemoryCompiler(l_WasmAssemblyName, l_WasmMetaData);
        l_Compiler.Compile();
        Console.WriteLine($"IL generated successfully");

        using (MemoryStream l_TempAssemblyStream = l_Compiler.CreateAssemblyStream())
        {
            if (Directory.Exists(p_OutputDir))
            {
                // Ensure the output directory is empty
                Directory.Delete(p_OutputDir, true);
                Directory.CreateDirectory(p_OutputDir);
            }
            else
            {
                // Create the output directory if it does not exist
                Directory.CreateDirectory(p_OutputDir);
            }


            // Reset stream position to the beginning
            l_TempAssemblyStream.Position = 0;

            // Load the assembly from the stream
            using (var l_PeFile = new PEFile("Dynamic", l_TempAssemblyStream, PEStreamOptions.PrefetchEntireImage))
            {
                DecompilerSettings l_Settings = new DecompilerSettings(LanguageVersion.Latest);

                var l_Resolver = new UniversalAssemblyResolver("Dynamic", false,
                    l_PeFile.DetectTargetFrameworkId(), l_PeFile.DetectRuntimePack(),
                    l_Settings.LoadInMemory ? PEStreamOptions.PrefetchMetadata : PEStreamOptions.Default,
                    l_Settings.ApplyWindowsRuntimeProjections ? MetadataReaderOptions.ApplyWindowsRuntimeProjections : MetadataReaderOptions.None);
                
                var l_Decompiler = new CSharpDecompiler(l_PeFile, l_Resolver, l_Settings);

                foreach (var l_Type in l_PeFile.Metadata.TypeDefinitions)
                {
                    var l_TypeDef = l_PeFile.Metadata.GetTypeDefinition(l_Type);
                    string l_Ns = l_PeFile.Metadata.GetString(l_TypeDef.Namespace);
                    string l_Name = l_PeFile.Metadata.GetString(l_TypeDef.Name);

                    // Skip <Module>
                    if (l_Name == "<Module>")
                        continue;

                    string l_FileName = string.IsNullOrEmpty(l_Ns)
                        ? l_Name + ".cs"
                        : l_Ns.Replace('.', '_') + "_" + l_Name + ".cs";

                    string l_FilePath = Path.Combine(p_OutputDir, l_FileName);

                    Console.WriteLine($"Writing '{l_FilePath}'...");

                    string l_Code = l_Decompiler.DecompileTypeAsString(l_Type.GetFullTypeName(l_PeFile.Metadata));

                    using (FileStream l_OutputStream = new FileStream(l_FilePath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        using (StreamWriter l_StreamWriter = new StreamWriter(l_OutputStream, Encoding.UTF8))
                        {
                            await l_StreamWriter.WriteAsync(l_Code);
                        }
                    }
                }
            }

            Console.WriteLine($"Decompiled source written to '{p_OutputDir}'");
        }
    }
}