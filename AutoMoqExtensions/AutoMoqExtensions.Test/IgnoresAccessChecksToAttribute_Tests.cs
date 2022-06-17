using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.Test
{
    internal class IgnoresAccessChecksToAttribute_Tests
    {
        [Test]
        public void Test_IgnoresAccessChecksToAttribute_HasCorrectNamespace() // I originally had an issue with this...
        {
            typeof(IgnoresAccessChecksToAttribute).Namespace.Should().Be("System.Runtime.CompilerServices");
        }
    }
}
