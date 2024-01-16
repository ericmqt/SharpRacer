using System.Diagnostics.CodeAnalysis;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Sdk;

namespace SharpRacer.SourceGenerators.Testing.Syntax;
public class SyntaxNodeDifference
{
    public SyntaxNodeDifference(SyntaxNode leftNode, SyntaxNode rightNode, SyntaxNodeDifferenceType differenceType)
    {
        LeftNode = leftNode;
        RightNode = rightNode;
        DifferenceType = differenceType;
    }

    public SyntaxNodeDifferenceType DifferenceType { get; }
    public SyntaxNode LeftNode { get; }
    public SyntaxNode RightNode { get; }

    public XunitException ToXunitException(string leftNodeArgumentName, string rightNodeArgumentName)
    {
        if (DifferenceType == SyntaxNodeDifferenceType.ChildNodeCount)
        {
            var msg = new StringBuilder()
                .AppendLine("Syntax nodes have different child node counts:")
                .AppendLine($"{leftNodeArgumentName} @ {LeftNode.Span}: {LeftNode.Kind()} (children: {LeftNode.ChildNodes().Count()})")
                .AppendLine($"{rightNodeArgumentName} @ {RightNode.Span}: {RightNode.Kind()} (children: {RightNode.ChildNodes().Count()})");

            return new XunitException(msg.ToString());
        }
        else if (DifferenceType == SyntaxNodeDifferenceType.SyntaxKind)
        {
            var msg = new StringBuilder()
                .AppendLine("Syntax nodes have different SyntaxKind values:")
                .AppendLine($"{leftNodeArgumentName} @ {LeftNode.Span}: {LeftNode.Kind()}")
                .AppendLine($"{rightNodeArgumentName} @ {RightNode.Span}: {RightNode.Kind()}");

            return new XunitException(msg.ToString());
        }

        var defaultMsg = new StringBuilder()
            .AppendLine($"{nameof(SyntaxNode.IsEquivalentTo)} returned false:")
            .AppendLine($"{leftNodeArgumentName} @ {LeftNode.Span}: {LeftNode.Kind()}")
            .AppendLine($"{rightNodeArgumentName} @ {RightNode.Span}: {RightNode.Kind()}");

        return new XunitException(defaultMsg.ToString());
    }

    public static bool TryFindFirst(SyntaxNode left, SyntaxNode right, [NotNullWhen(true)] out SyntaxNodeDifference? difference)
    {
        if (left.IsEquivalentTo(right))
        {
            difference = null;
            return false;
        }

        var leftKind = left.Kind();
        var rightKind = right.Kind();

        if (leftKind != rightKind)
        {
            difference = new SyntaxNodeDifference(left, right, SyntaxNodeDifferenceType.SyntaxKind);

            return true;
        }

        var leftChildren = left.ChildNodes().ToArray();
        var rightChildren = right.ChildNodes().ToArray();

        if (leftChildren.Length == 0 && rightChildren.Length == 0)
        {
            difference = new SyntaxNodeDifference(left, right, SyntaxNodeDifferenceType.Equivalency);

            return true;
        }

        if (leftChildren.Length != rightChildren.Length)
        {
            difference = new SyntaxNodeDifference(left, right, SyntaxNodeDifferenceType.ChildNodeCount);

            return true;
        }

        for (int i = 0; i < leftChildren.Length; i++)
        {
            var childLeft = leftChildren[i];
            var childRight = rightChildren[i];

            if (!childLeft.IsEquivalentTo(childRight))
            {
                if (TryFindFirst(childLeft, childRight, out var innerDifference))
                {
                    difference = innerDifference;
                    return true;
                }
            }
        }

        difference = null;
        return false;
    }
}
