using System;
using System.Collections.Generic;

namespace WebAssemblySharp.Runtime.JIT;

public class WebAssemblyJitStackFrame
{
    private WebAssemblyJitValue[] m_Locals;
    
    public WebAssemblyJitStackFrame(WebAssemblyJitValue[] p_Local)
    {
        m_Locals = p_Local;
    }
    
    public WebAssemblyJitValue GetLocal(uint p_Index)
    {
        return m_Locals[p_Index];
    }
    
}