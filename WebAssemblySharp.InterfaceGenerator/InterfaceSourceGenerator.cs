using System;
using System.Collections;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Mono.Cecil;
using WebAssemblySharp.Attributes;
using WebAssemblySharp.MetaData;
using WebAssemblySharp.MetaData.Utils;
using WebAssemblySharp.Readers.Binary;
using WebAssemblySharp.Runtime.Utils;
using ModuleDefinition = Mono.Cecil.ModuleDefinition;

namespace WebAssemblySharp.InterfaceGenerator;

[Generator]
public class InterfaceSourceGenerator: IIncrementalGenerator
{
    private static readonly String m_DefinitionAttributeName = nameof(WebAssemblyModuleDefinitionAttribute);
    private static readonly String m_DefinitionAttributeNameShort = nameof(WebAssemblyModuleDefinitionAttribute).Replace("Attribute", "");
    
    private static readonly String m_ManifestResourceAttributeName = nameof(WebAssemblyModuleManifestResource);
    private static readonly String m_ManifestResourceAttributeNameShort = nameof(WebAssemblyModuleManifestResource).Replace("Attribute", "");
    
    public void Initialize(IncrementalGeneratorInitializationContext p_Context)
    {
        
        IncrementalValuesProvider<InterfaceInfo> l_CalculatorClassesProvider = p_Context.SyntaxProvider.CreateSyntaxProvider(
            predicate: (SyntaxNode p_Node, CancellationToken p_CancelToken) =>
            {
                //the predicate should be super lightweight so it can quickly filter out nodes that are not of interest
                //it is basically called all of the time so it should be a quick filter
                return p_Node is InterfaceDeclarationSyntax l_InterfaceDeclarationSyntax && l_InterfaceDeclarationSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)) && HasAttribute(l_InterfaceDeclarationSyntax);
            },
            transform: (GeneratorSyntaxContext p_Ctx, CancellationToken p_CancelToken) =>
            {
                
                return Transform(p_Ctx, p_CancelToken);
                
            }
        );
        
        
        //next, we register the Source Output to call the Execute method so we can do something with these filtered items
        p_Context.RegisterSourceOutput(l_CalculatorClassesProvider, (p_SourceProductionContext, p_InterfaceClass) 
            => Execute(p_InterfaceClass, p_SourceProductionContext));
        
    }

    private InterfaceInfo Transform(GeneratorSyntaxContext p_Ctx, CancellationToken p_CancelToken)
    {
        InterfaceDeclarationSyntax l_InterfaceClass = (InterfaceDeclarationSyntax)p_Ctx.Node;
        
        string l_NameSpace = GetNamespace(l_InterfaceClass);

        if (String.IsNullOrEmpty(l_NameSpace))
            throw new Exception("No namespace found for interface " + l_InterfaceClass.Identifier.ToString());;

        p_CancelToken.ThrowIfCancellationRequested();
        
        WasmMetaData l_MetaData = LaodWasmMetaData(l_InterfaceClass, p_Ctx.SemanticModel, p_CancelToken);

        if (l_MetaData == null)
            throw new Exception("No metadata found for interface " + l_InterfaceClass.Identifier.ToString());
        
        p_CancelToken.ThrowIfCancellationRequested();
        
        return new InterfaceInfo() {
            Name = l_InterfaceClass.Identifier.ToString(),
            Namespace = l_NameSpace,
            MetaData = l_MetaData
        };
    }

    private WasmMetaData LaodWasmMetaData(BaseTypeDeclarationSyntax p_InterfaceClass, SemanticModel p_CtxSemanticModel, CancellationToken p_CancelToken)
    {
        AttributeSyntax l_Attribute = p_InterfaceClass.AttributeLists
            .SelectMany(l_AttributeList => l_AttributeList.Attributes)
            .FirstOrDefault(l_Attribute => l_Attribute.Name.ToString() == m_ManifestResourceAttributeName || l_Attribute.Name.ToString() == m_ManifestResourceAttributeNameShort);


        if (l_Attribute != null)
        {
            string l_Location = null;
            IAssemblySymbol l_AssemblyInfo = null;
            
            // ManifestResource
            if (l_Attribute.ArgumentList.Arguments.Count == 2)
            {
                l_Location = l_Attribute.ArgumentList.Arguments[0].Expression.ToString().Trim('"');
                TypeSyntax l_AssemblyTypeSyntax = ((TypeOfExpressionSyntax)l_Attribute.ArgumentList.Arguments[1].Expression).Type;
                l_AssemblyInfo = p_CtxSemanticModel.GetTypeInfo(l_AssemblyTypeSyntax).Type.ContainingAssembly;
            }
            else if (l_Attribute.ArgumentList.Arguments.Count == 1)
            {
                l_Location = l_Attribute.ArgumentList.Arguments[0].Expression.ToString().Trim('"');
            }

            if (l_Location == null)
                return null;
            
            if (l_AssemblyInfo != null)
            {
            
                ImmutableArray<MetadataReference> l_References = p_CtxSemanticModel.Compilation.ExternalReferences;

                foreach (MetadataReference l_Reference in l_References)
                {
                    p_CancelToken.ThrowIfCancellationRequested();
                    
                    if (l_Reference is PortableExecutableReference l_PortableExecutableReference)
                    {
                        string l_AssemblyPath = l_PortableExecutableReference.FilePath;
                        string l_AssemblyName = Path.GetFileNameWithoutExtension(l_AssemblyPath);
                        
                        if (l_AssemblyInfo.Name == l_AssemblyName)
                        {
                            ModuleDefinition l_Definition = Mono.Cecil.ModuleDefinition.ReadModule(l_AssemblyPath);

                            foreach (Resource l_Resource in l_Definition.Resources)
                            {
                                if (l_Resource.Name == l_Location)
                                {
                                    if (l_Resource is Mono.Cecil.EmbeddedResource l_EmbeddedResource)
                                    {
                                        using (Stream l_Stream = l_EmbeddedResource.GetResourceStream())
                                        {
                                            WasmBinaryReader l_BinaryReader = new WasmBinaryReader();
                                            byte[] l_Buffer = new byte[1024];
                                            
                                            int l_Read = l_Buffer.Length;
                                            
                                            while (l_Read == l_Buffer.Length)
                                            {
                                                p_CancelToken.ThrowIfCancellationRequested();
                                                l_Read = l_Stream.Read(l_Buffer, 0, l_Buffer.Length);
                                                l_BinaryReader.Read(new WebAssemblySharp.Polyfills.ReadOnlySpan<byte>(l_Buffer, 0, l_Read));
                                                
                                            }

                                            return l_BinaryReader.Finish();
                                        }
                                    }
                                }
                            }
                            
                        }
                        
                    }
                }
                
            }
        }
            
        return null;
    }

    private static bool HasAttribute(InterfaceDeclarationSyntax p_InterfaceDeclaration)
    {
        
        // Check each attribute list for the desired annotation
        foreach (var l_AttributeList in p_InterfaceDeclaration.AttributeLists)
        {
            
            foreach (var l_Attribute in l_AttributeList.Attributes)
            {
                var l_AttributeName = l_Attribute.Name.ToString();
                
                if (l_AttributeName == m_DefinitionAttributeName || l_AttributeName == m_DefinitionAttributeNameShort)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
    
    private void Execute(InterfaceInfo p_InterfaceClass, SourceProductionContext p_Context)
    {
        
        
        StringBuilder l_Code = new StringBuilder();
        l_Code.AppendLine("// <auto-generated/>");
        
        l_Code.AppendLine("using System;");
        l_Code.AppendLine("using System.Threading.Tasks;");
        l_Code.AppendLine("");
        l_Code.AppendLine("namespace " + p_InterfaceClass.Namespace + ";");
        l_Code.AppendLine("public partial interface " + p_InterfaceClass.Name);
        l_Code.AppendLine("{");

        foreach (WasmExport l_Export in p_InterfaceClass.MetaData.Export)
        {
            if (l_Export.Kind == WasmExternalKind.Function)
            {
                
                // Find the function index in the module
                int? l_FunctionIndex = WasmMetaDataUtils.FindExportIndex(p_InterfaceClass.MetaData, l_Export.Name.Value, l_Export.Kind);
                
                if (l_FunctionIndex == null)
                {
                    continue;
                }
                
                long l_FinalIndex = p_InterfaceClass.MetaData.FuncIndex[l_FunctionIndex.Value];
                WasmFuncType l_FuncType = p_InterfaceClass.MetaData.FunctionType[l_FinalIndex];
                
                if (l_FuncType.Results.Length > 1)
                    continue;

                l_Code.Append("   ");
                l_Code.Append("public ValueTask");

                if (l_FuncType.Results.Length == 1)
                {
                    l_Code.Append("<" + GetPrimitiveName(l_FuncType.Results[0]) + ">");
                }
                
                l_Code.Append(" " + l_Export.Name.Value + "(");
                
                for (int i = 0; i < l_FuncType.Parameters.Length; i++)
                {
                    string l_InternalType = GetPrimitiveName(l_FuncType.Parameters[i]);
                    l_Code.Append(l_InternalType + " p_" + "Value" + "_" + i);
                    
                    if (i < l_FuncType.Parameters.Length - 1)
                    {
                        l_Code.Append(", ");
                    }
                }
                
                l_Code.AppendLine(");");
                    
            }
            
        }
        
        l_Code.AppendLine("}");
        
        
        p_Context.AddSource(p_InterfaceClass.Namespace + "." + p_InterfaceClass.Name + ".Generated", l_Code.ToString());
    }

    private string GetPrimitiveName(WasmDataType p_WasmDataType)
    {
        switch (p_WasmDataType)
        {
            case WasmDataType.I32:
                return "int";
            case WasmDataType.I64:
                return "long";
            case WasmDataType.F32:
                return "float";
            case WasmDataType.F64:
                return "double";
            default:
                throw new ArgumentOutOfRangeException(nameof(p_WasmDataType), p_WasmDataType, null);
        }
    }


    public string GetNamespace(BaseTypeDeclarationSyntax p_Syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string l_NameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode l_PotentialNamespaceParent = p_Syntax.Parent;

        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (l_PotentialNamespaceParent != null &&
               !(l_PotentialNamespaceParent is NamespaceDeclarationSyntax)
               && !(l_PotentialNamespaceParent is FileScopedNamespaceDeclarationSyntax))
        {
            l_PotentialNamespaceParent = l_PotentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (l_PotentialNamespaceParent is BaseNamespaceDeclarationSyntax l_NamespaceParent)
        {
            // We have a namespace. Use that as the type
            l_NameSpace = l_NamespaceParent.Name.ToString();

            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (!(l_NamespaceParent.Parent is NamespaceDeclarationSyntax parent))
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                l_NameSpace = $"{l_NamespaceParent.Name}.{l_NameSpace}";
                l_NamespaceParent = parent;
            }
        }

        // return the final namespace
        return l_NameSpace;
    }
    
    private struct InterfaceInfo
    {
        public string Name { get; set; }
        public string Namespace { get; set; }
        public WasmMetaData MetaData { get; set; }
        
    }
    
}