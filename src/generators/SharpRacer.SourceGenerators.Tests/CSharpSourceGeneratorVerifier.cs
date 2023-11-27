using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;

namespace SharpRacer.SourceGenerators.Tests;

public static class CSharpSourceGeneratorVerifier<TSourceGenerator>
    where TSourceGenerator : ISourceGenerator, new()
{
    public class Test : CSharpSourceGeneratorTest<TSourceGenerator, XUnitVerifier>
    {
        public Test()
        {

        }
    }
}
