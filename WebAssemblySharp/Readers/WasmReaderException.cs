using System;

namespace WebAssemblySharp.Readers;

public class WasmReaderException: Exception
{
    public WasmReaderException(string? message) : base(message)
    {
    }

    public WasmReaderException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}