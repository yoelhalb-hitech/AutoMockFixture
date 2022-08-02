using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils
{
    public class AutoMockTypeControl
    {
        public List<Type> AlwaysAutoMockTypes { get; set; } = new List<Type>();
        public List<Type> NeverAutoMockTypes { get; set; } = new List<Type>();
    }
}
