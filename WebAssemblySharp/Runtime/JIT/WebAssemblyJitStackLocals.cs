using System;
using System.Collections.Generic;

namespace WebAssemblySharp.Runtime.JIT;

public readonly struct WebAssemblyJitStackLocals
{
    private readonly WebAssemblyJitValue[] m_Locals;
    
    public WebAssemblyJitStackLocals(WebAssemblyJitValue[] p_Local)
    {
        m_Locals = p_Local;
    }
    
    public ref WebAssemblyJitValue GetLocal(uint p_Index)
    {
        return ref m_Locals[p_Index];
    }

    public WebAssemblyJitValue[] GetBuffer()
    {
        return m_Locals;
    }
    
}