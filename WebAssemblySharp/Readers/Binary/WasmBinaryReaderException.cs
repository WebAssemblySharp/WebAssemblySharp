using System;

namespace WebAssemblySharp.Readers.Binary;

public class WasmBinaryReaderException: WasmReaderException
{
    public WasmBinaryReaderException(string p_Message) : base(p_Message)
    {
    }

    public WasmBinaryReaderException(string p_Message, Exception p_InnerException) : base(p_Message, p_InnerException)
    {
    }
}