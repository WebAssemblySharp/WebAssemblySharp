/**
 * Args for WebAssemblySharp.Tool
 * PathToWasmFile: The path to the WebAssembly file to be processed.
 * OutputPath: The name of the output module.
 *
 */

using System;
using WebAssemblySharp.Tool;

if (args.Length < 2)
{
    Console.WriteLine("Usage: WebAssemblySharp.Tool <PathToWasmFile> <OutputFileName>");
    return;
}

string l_PathToWasmFile = args[0];
string l_OutputDir = args[1];

if (!System.IO.File.Exists(l_PathToWasmFile))
{
    Console.WriteLine($"Error: The file '{l_PathToWasmFile}' does not exist.");
    return;
}

try
{
    await SourceCodeGenerator.Generate(l_PathToWasmFile, l_OutputDir);
}
catch (Exception e)
{
    Console.WriteLine(e);
}