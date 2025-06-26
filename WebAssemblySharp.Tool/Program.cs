using System.CommandLine;
using WebAssemblySharp.Tool;

/**
 * Args for WebAssemblySharp.Tool
 * PathToWasmFile: The path to the WebAssembly file to be processed.
 * OutputPath: The name of the output module.
 *
 */

RootCommand l_Command = new RootCommand("WebAssemblySharp.Tool");
l_Command.Description = "A tool to work with WebAssembly.";

GenerateCommandSetup.Setup(l_Command);

await l_Command.InvokeAsync(args);