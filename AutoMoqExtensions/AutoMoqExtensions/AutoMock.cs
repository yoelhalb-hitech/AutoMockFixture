﻿using AutoMoqExtensions.AutoMockUtils;
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

        private static object castleProxyFactoryInstance;
        private static FieldInfo generatorFieldInfo;
        static AutoMock()
        {
            var moqAssembly = typeof(Mock).Assembly;

            var proxyFactoryType = moqAssembly.GetType("Moq.ProxyFactory");
            castleProxyFactoryInstance = proxyFactoryType.GetProperty("Instance").GetValue(null);

            var castleProxyFactoryType = moqAssembly.GetType("Moq.CastleProxyFactory");
            generatorFieldInfo = castleProxyFactoryType.GetField("generator", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public override bool CallBase { get => base.CallBase; set
              {
                if (mocked is not null) throw new Exception("Cannot set callbase after object has been created");
                base.CallBase = value;
            } }
        private void SetupGenerator()
            => generatorFieldInfo.SetValue(castleProxyFactoryInstance, new AutoMockProxyGenerator(target, this.CallBase));
        private T? target;
        public bool TrySetTarget(T target)
        {
            if (mocked is not null || this.target is not null) return false;

            SetTarget(target);
            return true;
        }
        public void SetTarget(T target)
        {
            if (mocked is not null) throw new Exception("Cannot set target when object is already created");
            if (this.target is not null) throw new Exception("Can only set target once");

            this.target = target;
        }

        private T? mocked;
        public Type GetInnerType() => typeof(T);
        public void EnsureMocked()
        {
            if (mocked is null)
            {
                // The generator is static so we have to reduce it to the minimum
                SetupGenerator();
                mocked = base.Object;
                this.target = null;
                SetupGenerator();
            } 
        }
        object IAutoMock.GetMocked() => GetMocked();
        public T GetMocked()
        {
            EnsureMocked();

            return mocked!;
        }

        public static implicit operator T(AutoMock<T> m) => m.GetMocked();

        public AutoMock(MockBehavior behavior) : base(behavior) { setupUtils = new SetupUtils<T>(this); SetupGenerator(); }
        public AutoMock(params object?[] args) : base(args) { setupUtils = new SetupUtils<T>(this); SetupGenerator(); }
        public AutoMock(MockBehavior behavior, params object?[] args) : base(behavior, args)
        {
            setupUtils = new SetupUtils<T>(this);
            SetupGenerator();
        }
        public AutoMock() : base() { setupUtils = new SetupUtils<T>(this); SetupGenerator(); }

        public new void VerifyAll()
        {
            VerifyList.ForEach(v => v.Verify(this));// TODO... maybe we should catch everything and rethrow as aggregate exception
            base.VerifyAll();
        }

        private readonly SetupUtils<T> setupUtils;
    }
}
