using System.Runtime.CompilerServices;

namespace AutoMoqExtensions.Test;

internal class IgnoresAccessChecksToAttribute_Tests
{
    [Test]
    public void Test_IgnoresAccessChecksToAttribute_HasCorrectNamespace() // I originally had an issue with this...
    {
        typeof(IgnoresAccessChecksToAttribute).Namespace.Should().Be("System.Runtime.CompilerServices");
    }
}
