using System;

namespace WebAssemblySharp.Runtime.Interpreter;

public class WebAssemblyInterpreterException: Exception
{
    public WebAssemblyInterpreterException(string p_Message) : base(p_Message)
    {
    }

    public WebAssemblyInterpreterException(string p_Message, Exception p_InnerException) : base(p_Message, p_InnerException)
    {
    }
}