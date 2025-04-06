using WebAssemblySharp.MetaData.Instructions;

namespace WebAssemblySharp.Runtime.JIT;

public interface IWebAssemblyJitInteropOptimizer
{
    bool OptimizeInstruction(WasmInstruction p_Instruction);
}