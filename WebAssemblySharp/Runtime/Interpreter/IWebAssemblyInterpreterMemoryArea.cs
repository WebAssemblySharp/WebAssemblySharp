using System;

namespace WebAssemblySharp.Runtime.Interpreter;

/*
 * Defines an interface for accessing memory regions within a WebAssembly interpreter.
 * Provides controlled access to the underlying memory used during WebAssembly execution.
 */
public interface IWebAssemblyInterpreterMemoryArea
{
    Span<byte> GetMemoryAccess(int p_Address, int p_Length);
}