﻿using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.Moq4.AutoMockProxy;
using AutoMockFixture.Moq4.AutoMockUtils;
using AutoMockFixture.Moq4.VerifyInfo;
using Castle.DynamicProxy;
using SequelPay.DotNetPowerExtensions.Reflection;

namespace AutoMockFixture.Moq4;

public static class AutoMock
{
    public static T Of<T>() where T : class => new AutoMock<T>();
    public static AutoMock<T>? Get<T>(T? mocked) where T : class => AutoMockHelpers.GetAutoMock(mocked);
    public static bool IsAutoMock(object? obj) => new AutoMockHelpers().GetFromObj(obj) is not null;
}
public partial class AutoMock<T> : Mock<T>, IAutoMock, ISetCallBase where T : class
{
    public override T Object => GetMocked();
    public virtual ITracker? Tracker { get; set; }
    public virtual IAutoMockFixture Fixture => Tracker?.StartTracker.Fixture
            ?? throw new Exception($"Fixture not set, was this created by an `{nameof(IAutoMockFixture)}`?");
    public List<IVerifyInfo<T>> VerifyList { get; } = new List<IVerifyInfo<T>>();
    public Dictionary<string, MemberInfo> MethodsSetup { get; } = new Dictionary<string, MemberInfo>();
    public Dictionary<string, CannotSetupMethodException> MethodsNotSetup { get; }
                                        = new Dictionary<string, CannotSetupMethodException>();

    public override bool CallBase { get => base.CallBase; set
          {
            if (mocked is not null) throw new Exception("Cannot set callBase after object has been created");
            base.CallBase = value;
        } }

    public object? Target => target;

    void ISetCallBase.ForceSetCallBase(bool value) => base.CallBase = value;

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
    private static Type iautoMockedType = typeof(IAutoMocked);
    public void EnsureMocked()
    {
        if (mocked is not null) return;

        // Moq Bug Workaround
        // This has to be done otherwise explicit interface implementations and default interface implementations might not work correctly
        // While for callBase without a setup it will work correctly even without calling "As", it will not work correctly in the following situations:
        // 1) For non callBase without a setup
        // 2) For a custom setup if `As` has been called only after object creation
        // 3) When setting up via reflection
        var asMethod = typeof(Mock).GetMethod(nameof(Mock.As))!;
        var detailInfo = GetInnerType().GetTypeDetailInfo();
        var interfacesToFix = detailInfo.ExplicitMethodDetails.Select(m => m.ExplicitInterface)
                                .Union(detailInfo.ExplicitPropertyDetails.Select(m => m.ExplicitInterface))
                                .Union(detailInfo.ExplicitEventDetails.Select(e => e.ExplicitInterface));
        foreach (var iface in interfacesToFix.OfType<Type>())
        {
            try
            {
                if (!ProxyUtil.IsAccessible(iface)) continue; // Otherwise it will prevent it from creating the mocked object later

                asMethod.MakeGenericMethod(iface).Invoke(this, new Type[] { }); // This has to be done before creating the mocked object, otherwise it won't work
            }
            catch { } // TODO...
        }

        // The generator is static so we have to reduce it to the minimum
        // CAUTION: While it would have been a good idea to lock here we can't do it as this call is called recursively
        //      (since we mock the Type object in the generator) and we would end up with a deadlock
        try
        {
            this.AdditionalInterfaces.Add(iautoMockedType);
            GeneratorSetup.SetupGenerator(target, this.CallBase);

            mocked = base.Object;
        }
        finally
        {
            // We need to reset it in case the user wants to use the Mock directly as this property is static...
            // NOTE: Although this call is called recursively (in particular since we mock the Type object in the generator)
            //          we aren't concerned about the reset, since at the point it happens the generator was already called...
            GeneratorSetup.ResetGenerator();
            this.AdditionalInterfaces.Remove(iautoMockedType);
        }

        if (mocked is not null && this.target is not null && this.CallBase) SetupTargetMethods();
    }

    private void SetupTargetMethods()
    {
        var t = typeof(T);

        var existingSetups = Setups
                            .Select(s => s.GetType().GetProperty("Method")?.GetValue(s) as MethodInfo)
                            .Where(m => m is not null)
                            .ToList();

        //TODO... the whole idea is because we want to override the setup for HttpClient and we want to also Mock the `When` method by using the original as target

        //It appears that it doesn't setup abstract methods automatically for whatever reason
        var setupUtils = new SetupUtils<T>(this);
        var list = typeof(T).GetAllMethods()
            .Where(m => !m.IsPrivate && m.IsAbstract && !existingSetups.Any(s => m.IsEqual(s!)))
            .ToList();
        list.ForEach(m => setupUtils.SetupInternal(m, new { }, null, true));
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
    public AutoMock(MockBehavior behavior, params object?[] args) : base(behavior, args)
    {
        setupUtils = new SetupUtils<T>(this);
    }
    public AutoMock() : base() { setupUtils = new SetupUtils<T>(this); }

    void IAutoMock.Verify() => this.Verify();
    public new void Verify()
    {
        VerifyList.ForEach(v => v.Verify(this));// TODO... maybe we should catch everything and rethrow as aggregate exception
        base.Verify();
    }

    [Obsolete("Use Verify, as AutoFixture sets up everything")]
    public new void VerifyAll()
    {
        VerifyList.ForEach(v => v.Verify(this));// TODO... maybe we should catch everything and rethrow as aggregate exception
        base.VerifyAll();
    }

    public void Setup(string methodName, object args, object? result, Times? times = null)
    {
        var method = typeof(T).GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
        if (method == null)
        {
            throw new ArgumentException($"Method '{methodName}' not found on type {typeof(T).Name}");
        }

        if(this.CallBase && result is null) setupUtils.SetupInternal(method, args, times, callBase: true);
        else setupUtils.SetupInternal(method, args, result, times);
    }

    private readonly SetupUtils<T> setupUtils;
}
