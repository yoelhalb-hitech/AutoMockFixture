using AutoMoqExtensions.AutoMockUtils;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Test.AutoMockUtils
{
    internal class AutoMockHelpers_Tests
    {
        [Test]
        public void Test_GetFromObj_WorksWith_Delegate()
        {
            var mock = new AutoMock<Action>();

            AutoMockHelpers.GetFromObj(mock.Object).Should().NotBeNull();
        }
    }
}
