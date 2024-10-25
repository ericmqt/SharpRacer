namespace SharpRacer.SourceGenerators;
internal static class SystemIdentifiers
{
    static SystemIdentifiers()
    {
        SystemNamespace = new NamespaceIdentifier("System");
        SystemCodeDomCompilerNamespace = new NamespaceIdentifier("System.CodeDom.Compiler");
        SystemCollectionsGenericNamespace = new NamespaceIdentifier("System.Collections.Generic");

        // System
        ArgumentNullException = SystemNamespace.CreateType("ArgumentNullException");
        ObsoleteAttribute = SystemNamespace.CreateType("ObsoleteAttribute");

        // System.CodeDom.Compiler
        GeneratedCodeAttribute = SystemCodeDomCompilerNamespace.CreateType("GeneratedCodeAttribute");

        // System.Collections
        IEnumerable_T = SystemCollectionsGenericNamespace.CreateType("IEnumerable");
    }

    public static NamespaceIdentifier SystemNamespace { get; }
    public static NamespaceIdentifier SystemCollectionsGenericNamespace { get; }
    public static NamespaceIdentifier SystemCodeDomCompilerNamespace { get; }

    public static TypeIdentifier ArgumentNullException { get; }
    public static TypeIdentifier GeneratedCodeAttribute { get; }
    public static TypeIdentifier IEnumerable_T { get; }
    public static TypeIdentifier ObsoleteAttribute { get; }
}
