using System;
using System.CommandLine;

namespace WebAssemblySharp.Tool;

public static class GenerateCommandSetup
{
    public static void Setup(Command p_ParentCommand)
    {
        Command l_GenerateCommand = new Command("generate", "Generates the source code for a WebAssembly module.");
        
        Argument<string> l_PathToWasmFileArg = new Argument<string>("PathToWasmFile", "The path to the WebAssembly file to be processed.");
        Argument<string> l_OutFileNameArg = new Argument<string>("OutputFileName", "The name of the output module.");

        l_GenerateCommand.AddArgument(l_PathToWasmFileArg);
        l_GenerateCommand.AddArgument(l_OutFileNameArg);
        l_GenerateCommand.SetHandler(async (p_PathToWasmFile, p_OutputFileName) =>
        {
            if (!System.IO.File.Exists(p_PathToWasmFile))
            {
                Console.WriteLine($"Error: The file '{p_PathToWasmFile}' does not exist.");
                return;
            }
    
            try
            {
                await SourceCodeGenerator.Generate(p_PathToWasmFile, p_OutputFileName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }, l_PathToWasmFileArg, l_OutFileNameArg);
        
        p_ParentCommand.AddCommand(l_GenerateCommand);
    }
}