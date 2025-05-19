using System;

namespace WebAssemblySharp.Readers;

public class WasmReaderException: Exception
{
    public WasmReaderException(string p_Message) : base(p_Message)
    {
    }

    public WasmReaderException(string p_Message, Exception p_InnerException) : base(p_Message, p_InnerException)
    {
    }
}