using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.Interpreter;

public interface IWebAssemblyInterpreterInteropOptimizer
{
    bool OptimizeInstruction(WasmInstruction p_Instruction);
}