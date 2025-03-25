using System;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitException: Exception
{
    public WebAssemblyJitException(string p_Message) : base(p_Message)
    {
    }

    public WebAssemblyJitException(string p_Message, Exception p_InnerException) : base(p_Message, p_InnerException)
    {
    }
}