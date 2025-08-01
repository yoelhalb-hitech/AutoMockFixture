﻿using AutoMockFixture.FixtureUtils.Requests;
using AutoMockFixture.Moq4.FixtureUtils.Commands;
using AutoMockFixture.Moq4.FixtureUtils.Specifications;
using AutoMockFixture.Moq4.MockUtils;
using Castle.DynamicProxy;
using System.Collections;
using System.Collections.Concurrent;

namespace AutoMockFixture.Moq4.AutoMockUtils;

internal class AutoMockHelpers : IAutoMockHelpers
{
    public Type InterfaceProxyBase => typeof(global::Moq.Internals.InterfaceProxy);

    public IRequestSpecification MockRequestSpecification => new MockRequestSpecification();

    public static AutoMock<T>? GetAutoMock<T>(T? mocked) where T : class
    {
        if (mocked is null) return null;
        if(mocked is AutoMock<T> mock) return mock;
        if(mocked is IAutoMock) throw new ArgumentException("Object is already an AutoMock", nameof(mocked));

        try
        {
            var m = global::Moq.Mock.Get<T>(mocked);
            if(m is not IAutoMock) throw new ArgumentException("Object instance was created by Mock but not by AutoMockFixture.Moq.AutoMock. (Parameter 'mocked')");
            if(m is not AutoMock<T>) throw new ArgumentException($"Expected Mock to be of type `{typeof(AutoMock<T>).ToGenericTypeString()}` but found {m.GetType().ToGenericTypeString()}");

            return m as AutoMock<T>;
        }
        catch(ArgumentException ex) when (ex.Message == "Object instance was not created by Moq. (Parameter 'mocked')")
        {
            var obj = new AutoMockHelpers().GetFromObj(mocked);
            if(obj is not null) throw new InvalidCastException($"Mock is of type AutoMock<{obj.GetType().GetGenericArguments().First().ToGenericTypeString()}> and cannot be casted to AutoMock<{typeof(T).ToGenericTypeString()}>");

            throw new ArgumentException("Object instance was not created by AutoMockFixture.Moq.AutoMock. (Parameter 'mocked')", ex);
        }
    }

    public bool IsAutoMock<T>() => IsAutoMock(typeof(T));
    public bool IsAutoMock(Type? t) => t?.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(AutoMock<>);
    public Type? GetMockedType(Type? t) => t?.IsGenericType == true && t.GetGenericTypeDefinition() == typeof(AutoMock<>) ? t.GetTypeInfo().GetGenericArguments().Single() : null;
    /// <summary>
    /// Returns the non generic base interface, for use when it might be already an AutoMock but we don't know the generic type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public IAutoMock? GetFromObj(object? obj) => obj is IAutoMock m ? m : (obj as global::Moq.IMocked ?? (obj as Delegate)?.Target as global::Moq.IMocked)?.Mock as IAutoMock;
    public Type GetAutoMockType(Type inner) => typeof(AutoMock<>).MakeGenericType(inner);

    public bool IsAutoMockAllowed(Type t, bool force = false)
    {
        if (!Moq.Extensions.IsMockable(t) || t.IsValueType || (t.IsSealed && !typeof(System.Delegate).IsAssignableFrom(t))) return false;

        return IsAllowed(t, force);
    }

    public bool IsAllowed(Type t, bool force = false)
    {
        // We cannot afford to mock Type objects in the fixture as it might mess up things, although AutoMock itself allows it, and we actually use it
        // As a matter of fact there is no point to it either, as code using a type via a property or method etc. usually expects a specific type anyway
        if (t is null || typeof(Type).IsAssignableFrom(t)) return false;
        if(force) return true;

        if (t.IsPrimitive || t == typeof(string) || t == typeof(object) || (t.IsValueType && t.Namespace == nameof(System))
                    || t == typeof(Array)
                    ||  (t.IsGenericType && new[]
                        {
                            typeof(IEnumerable).Namespace,
                            typeof(List<>).Namespace,
                            typeof(ConcurrentDictionary<,>).Namespace,
                        }.Contains(t.GetGenericTypeDefinition().Namespace))
                    //|| typeof(IEnumerable).IsAssignableFrom(t)|| typeof(ICollection).IsAssignableFrom(t) || typeof(IList).IsAssignableFrom(t)

#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
                    || t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IAsyncEnumerable<>))
#endif

                    // This way we cover all different Tuple types...
                    || (t.Assembly == typeof(Tuple).Assembly) && t.FullName?.StartsWith(typeof(Tuple).FullName!) == true
                    || (t.Assembly == typeof(ValueTuple).Assembly) && t.FullName?.StartsWith(typeof(ValueTuple).FullName!) == true

                    || t == typeof(IntPtr) || t == typeof(UIntPtr)
                    || typeof(global::Moq.Mock).IsAssignableFrom(t)
                    || typeof(Type).IsAssignableFrom(t)
                    || t.Assembly == typeof(global::Moq.Mock).Assembly
                    || typeof(IFixture).IsAssignableFrom(t) || typeof(IAutoMock).IsAssignableFrom(t) || typeof(ITracker).IsAssignableFrom(t)
                    // TODO...have to figure out why it has a problem to mock it and hwo we can expect it in general
                    //     but maybe with our CustomMockVirtualMethodsCommand it is already fixed
                    || new[] { typeof(Assembly).Namespace, typeof(Thread).Namespace, typeof(Task).Namespace }.Contains(t.Namespace))
        {
            return false;
        }

        if (t.IsGenericType && new[]
{
            typeof(KeyValuePair<,>),
#if NET461_OR_GREATER || NETSTANDARD2_0_OR_GREATER
            typeof(IAsyncEnumerable<>),
#endif
            typeof(Nullable<>),
            typeof(AutoMock<>),
        }.Contains(t.GetGenericTypeDefinition()))
            return false;

        return true;
    }

    public SetupServiceFactoryBase GetSetupServiceFactory(Func<MethodSetupTypes> setupTypeFunc)
        => new SetupServiceFactory(setupTypeFunc);

    public ISpecimenCommand GetStubAllPropertiesCommand() => new AutoMockStubAllPropertiesCommand(this);

    public ISpecimenCommand GetClearInvocationsCommand() => new AutoMockClearInvocationsCommand(this);

    public ISpecimenCommand GetAutoMockInitCommand()  => new AutoMockInitCommand();

    public bool CanMock(Type t) => ProxyUtil.IsAccessible(t);
}
