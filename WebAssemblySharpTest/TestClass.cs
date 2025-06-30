using System;
using System.Threading.Tasks;

namespace WebAssemblySharpTest;

public class TestClass
{

    protected Func<String, String, String> m_Func;

    public void execute(String p_A, String p_B)
    {
        
        m_Func(p_A + p_B, p_B);
        
    }

}