using AutoMoqExtensions.AutoMockUtils;
using AutoMoqExtensions.FixtureUtils;
using AutoMoqExtensions.FixtureUtils.Requests;
using AutoMoqExtensions.MockUtils;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions
{
    public partial class AutoMock<T> : Mock<T>, IAutoMock where T : class
    {
        public override T Object => GetMocked();
        public virtual ITracker? Tracker { get; set; }
        public virtual AutoMockFixture Fixture => Tracker?.StartTracker.Fixture
                ?? throw new Exception($"Fixture not set, was this created by `{nameof(AutoMockFixture)}`?");
        public List<IVerifyInfo<T>> VerifyList { get; } = new List<IVerifyInfo<T>>();
        public Dictionary<string, MemberInfo> MethodsSetup { get; } = new Dictionary<string, MemberInfo>();
        public Dictionary<string, CannotSetupMethodException> MethodsNotSetup { get; }
                                            = new Dictionary<string, CannotSetupMethodException>();

        private T? mocked;
        public Type GetInnerType() => typeof(T);
        public void EnsureMocked()
        {
            if (mocked is null)
                mocked = base.Object;
        }
        object IAutoMock.GetMocked() => GetMocked();
        public T GetMocked()
        {
            EnsureMocked();

            return mocked!;
        }

        public static implicit operator T(AutoMock<T> m) => m.GetMocked();

        public AutoMock(MockBehavior behavior) : base(behavior) { setupUtils = new SetupUtils<T>(this); }
        public AutoMock(params object?[] args) : base(args) { setupUtils = new SetupUtils<T>(this); }
        public AutoMock(MockBehavior behavior, params object?[] args) : base(behavior, args) { setupUtils = new SetupUtils<T>(this); }
        public AutoMock() : base() { setupUtils = new SetupUtils<T>(this); }

        public new void VerifyAll()
        {
            VerifyList.ForEach(v => v.Verify(this));// TODO... maybe we should catch everything and rethrow as aggregate exception
            base.VerifyAll();
        }

        private readonly SetupUtils<T> setupUtils;
    }
}
