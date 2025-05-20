using System;

namespace WebAssemblySharp.Attributes;

public class WebAssemblyModuleManifestResource: Attribute
{
    public String Location { get; }
    public Type AssemblyType { get;  }

    public WebAssemblyModuleManifestResource(string p_Location)
    {
        Location = p_Location;
    }

    public WebAssemblyModuleManifestResource(string p_Location, Type p_AssemblyType)
    {
        Location = p_Location;
        AssemblyType = p_AssemblyType;
    }
}