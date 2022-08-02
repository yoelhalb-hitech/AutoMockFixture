using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.FixtureUtils.Requests.MainRequests;
using System.Collections;
using static AutoMoqExtensions.FixtureUtils.Builders.SpecialBuilders.AutoMockTypeControlBuilder;

namespace AutoMoqExtensions.Test.FixtureUtils.Builders.SpecialBuilders;

internal class AutoMockTypeControlBuilder_Tests
{
    internal class TypeControlHelper_Tests
    {
        private TypeControlHelper GetTypeControlHelper(Type requestType, Type innerType)
        {
            var fixture = new AutoMockFixture();

            var recursionContext = new RecursionContext(fixture, fixture);

            var request = GetRequest(requestType, innerType, fixture);

            return new TypeControlHelper(fixture, request, innerType, recursionContext);
        }

        private IRequestWithType GetRequest(Type requestType, Type innerType, AutoMockFixture fixture)
            => requestType switch
            {
                Type t when t == typeof(AutoMockRequest) => new AutoMockRequest(innerType, fixture),
                Type t when t == typeof(AutoMockDependenciesRequest) => new AutoMockDependenciesRequest(innerType, fixture),
                Type t when t == typeof(NonAutoMockRequest) => new NonAutoMockRequest(innerType, fixture),
                Type t when t == typeof(AutoMockDirectRequest) => new AutoMockDirectRequest(innerType, fixture),
                _ => throw new NotSupportedException(),
            };


        [Test]
        [TestCase(typeof(AutoMockRequest))]
        [TestCase(typeof(AutoMockDependenciesRequest))]
        [TestCase(typeof(NonAutoMockRequest))]
        [TestCase(typeof(AutoMockDirectRequest))]
        public void Test_Returns_Null_When_NoMatch(Type requestType)
        {
            var type = typeof(Task);
            var typeControl = new AutoMockTypeControl { AlwaysAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(requestType, typeof(IEnumerable));
            var request = helper.GetRequest(typeControl);

            request.Should().BeNull();
        }

        [Test]
        public void Test_Returns_Null_When_AutoMockDirectRequest_And_AlwaysAutoMockTypes_IsMatch()
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { AlwaysAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(typeof(AutoMockDirectRequest), type);
            var request = helper.GetRequest(typeControl);

            request.Should().BeNull();
        }

        [Test]
        public void Test_Returns_Null_When_AutoMockDirectRequest_And_NeverAutoMockTypes_IsMatch()
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { NeverAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(typeof(AutoMockDirectRequest), type);
            var request = helper.GetRequest(typeControl);

            request.Should().BeNull();
        }

        [Test]
        public void Test_Returns_Null_When_AutoMockRequest_And_AlwaysAutoMockTypes_IsMatch()
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { AlwaysAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(typeof(AutoMockRequest), type);
            var request = helper.GetRequest(typeControl);

            request.Should().BeNull();
        }

        [Test]
        [TestCase(typeof(AutoMockDependenciesRequest))]
        [TestCase(typeof(NonAutoMockRequest))]
        public void Test_Returns_AutoMockRequest_When_Not_AutoMockRequest_And_AlwaysAutoMockTypes_IsMatch(
                                                Type requestType)
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { AlwaysAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(requestType, type);
            var request = helper.GetRequest(typeControl);

            request.Should().NotBeNull();
            request.Should().BeOfType<AutoMockRequest>();
        }

        [Test]
        [TestCase(typeof(AutoMockDependenciesRequest), false)]
        [TestCase(typeof(NonAutoMockRequest), true)]
        public void Test_SetsCorrectly_NoMockDependencies_When_Not_AutoMockRequest_And_AlwaysAutoMockTypes_IsMatch(
            Type requestType, bool noMockDependencies)
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { AlwaysAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(requestType, type);
            var request = helper.GetRequest(typeControl);
            
            request!.Should().BeOfType<AutoMockRequest>();
            ((AutoMockRequest)request!).NoMockDependencies.Should().Be(noMockDependencies);
        }

        [Test]
        [TestCase(typeof(AutoMockDependenciesRequest))]
        [TestCase(typeof(NonAutoMockRequest))]
        public void Test_Returns_Null_When_Not_AutoMockRequest_And_NeverAutoMockTypes_IsMatch(Type requestType)
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { NeverAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(requestType, type);
            var request = helper.GetRequest(typeControl);

            request.Should().BeNull();
        }

        [Test]
        public void Test_Returns_AutoMockDependenciesRequest_When_AutoMockRequest_And_NeverAutoMockTypes_IsMatch()
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { NeverAutoMockTypes = new[] { type }.ToList() };

            var helper = GetTypeControlHelper(typeof(AutoMockRequest), type);
            var request = helper.GetRequest(typeControl);

            request.Should().BeOfType<AutoMockDependenciesRequest>();
        }

        [Test]
        public void Test_Returns_NonAutoMock_When_AutoMockRequest_NoMockDependencies_And_NeverAutoMockTypes_IsMatch()
        {
            var type = typeof(IEnumerable);
            var typeControl = new AutoMockTypeControl { NeverAutoMockTypes = new[] { type }.ToList() };

            var fixture = new AutoMockFixture();
            var recursionContext = new RecursionContext(fixture, fixture);

            var request = new AutoMockRequest(type, fixture) { NoMockDependencies = true };

            var helper = new TypeControlHelper(fixture, request, type, recursionContext);

            var newRequest = helper.GetRequest(typeControl);

            newRequest.Should().BeOfType<NonAutoMockRequest>();
        }
    }
}
