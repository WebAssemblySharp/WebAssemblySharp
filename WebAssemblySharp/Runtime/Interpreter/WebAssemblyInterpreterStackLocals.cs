namespace WebAssemblySharp.Runtime.Interpreter;

public readonly struct WebAssemblyInterpreterStackLocals
{
    private readonly WebAssemblyInterpreterValue[] m_Locals;
    
    public WebAssemblyInterpreterStackLocals(WebAssemblyInterpreterValue[] p_Local)
    {
        m_Locals = p_Local;
    }
    
    public ref WebAssemblyInterpreterValue GetLocal(uint p_Index)
    {
        return ref m_Locals[p_Index];
    }

    public WebAssemblyInterpreterValue[] GetBuffer()
    {
        return m_Locals;
    }
    
}