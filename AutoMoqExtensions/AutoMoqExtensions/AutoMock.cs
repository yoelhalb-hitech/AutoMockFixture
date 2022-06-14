using AutoMoqExtensions.Expressions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Moq;
using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace AutoMoqExtensions
{
    {
        public List<IVerifyInfo<T>> VerifyList { get; } = new List<IVerifyInfo<T>>();
        private T? mocked;
        public Type GetInnerType() => typeof(T);
        public void EnsureMocked()
        {
            if (mocked is null)
                mocked = this.Object;
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
