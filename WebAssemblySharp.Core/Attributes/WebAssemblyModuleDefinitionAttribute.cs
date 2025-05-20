using System;

namespace WebAssemblySharp.Attributes;

[AttributeUsage(AttributeTargets.Interface)]
public class WebAssemblyModuleDefinitionAttribute: Attribute
{
    public string Name { get; }
    
    public WebAssemblyModuleDefinitionAttribute(string p_Name)
    {
        Name = p_Name;
    }
}