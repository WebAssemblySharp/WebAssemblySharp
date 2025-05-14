using System.Linq;
using System.Reflection;

namespace WebAssemblySharpTest.Runtime;

public class DebugUtils
{
    public static void DumpDynamicAssembly(string p_DumpPath)
    {
        Assembly l_Assembly = System.AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(x => x.GetName().Name == "DynamicAssembly");
        Assert.IsNotNull(l_Assembly);
        var generator = new Lokad.ILPack.AssemblyGenerator();
        generator.GenerateAssembly(l_Assembly, p_DumpPath);
    }
}