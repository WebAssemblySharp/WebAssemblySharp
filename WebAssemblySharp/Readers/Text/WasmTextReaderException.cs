using System;

namespace WebAssemblySharp.Readers.Text;

public class WasmTextReaderException: WasmReaderException
{
    public WasmTextReaderException(string p_Message) : base(p_Message)
    {
    }

    public WasmTextReaderException(string p_Message, Exception p_InnerException) : base(p_Message, p_InnerException)
    {
    }
}