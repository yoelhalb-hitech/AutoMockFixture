using AutoMockFixture.Extensions;

namespace AutoMockFixture.Moq4;

public static class AutoMockFixtureExtensions
{ 

    #region Getters

    public static AutoMock<T> GetAutoMock<T>(this IAutoMockFixture fixture, object obj, string path) where T : class
    {
        var result = fixture.GetAutoMock(obj, path);
        if (result is not AutoMock<T> mock)
            throw new Exception($"Result object is `{nameof(AutoMock<object>)}` and not `{nameof(AutoMock<object>)}<{typeof(T).Name}>`");

        return mock;
    }


    public static IEnumerable<AutoMock<T>> GetAutoMocks<T>(this IAutoMockFixture fixture, object obj) where T : class 
                                => fixture.GetAutoMocks(obj, typeof(T)).OfType<AutoMock<T>>();
 
    public static AutoMock<T> GetAutoMock<T>(this IAutoMockFixture fixture, object obj) where T : class 
            => fixture.GetAutoMocks<T>(obj).SingleOrDefault() ?? throw new Exception("AutoMock not found or is garbage collected");

    public static IEnumerable<AutoMock<T>> GetAutoMocks<T>(this IAutoMockFixture fixture, bool freezeAndCreate = false) where T : class
    {
        var existing = fixture.TrackerDict.Keys.SelectMany(k => fixture.GetAutoMocks(k, typeof(T)).OfType<AutoMock<T>>());
        if (!existing.Any() && freezeAndCreate)
        {
            fixture.Freeze<T>();
            var newMock = fixture.Create<AutoMock<T>>();
            if (newMock is not null) return new AutoMock<T>[] { newMock };
        }

        return existing;
    }

    public static AutoMock<T> GetAutoMock<T>(this IAutoMockFixture fixture, bool freezeAndCreate = false) where T : class
                            => fixture.GetAutoMocks<T>(freezeAndCreate).SingleOrDefault()
                                ?? throw new Exception("AutoMock not found or is garbage collected");

    public static AutoMock<T> On<T>(this IAutoMockFixture fixture) where T : class => fixture.GetAutoMock<T>(true);

    public static T Object<T>(this IAutoMockFixture fixture) where T : class => fixture.GetAutoMock<T>(true).Object;

    #endregion

    #region Verify

    // NOTE: We don't do VerifyAll() as it will try to verify all setups that the AutoMockFixture has done
    public static void Verify(this IAutoMockFixture fixture, object obj)
    {
        if (!fixture.MocksDict.Any(d => d.Key.Target == obj)) throw new Exception("Object not found, ensure that it is a root object in the current fixture that is not yet garbage collected, and possibly verify that .Equals() works correctly on the object");

        if (fixture.AutoMockHelpers.GetFromObj(obj) is IAutoMock mock) mock.Verify();
        fixture.MocksDict.First(d => d.Key.Target == obj).Value?.Result.GetValidValues().ForEach(m => m.Verify());
    }

    public static void Verify(this IAutoMockFixture fixture)
    {
        fixture.TrackerDict.Keys.ToList().ForEach(k => fixture.Verify(k));
    }
    
    public static void VerifyNoOtherCalls(this IAutoMockFixture fixture, object obj)
    {
        if (!fixture.MocksDict.Any(d => d.Key.Target == obj)) throw new Exception("Object not found, ensure that it is a root object in the current fixture, and possibly verify that .Equals() works correctly on the object");

        if (fixture.AutoMockHelpers.GetFromObj(obj) is IAutoMock mock) mock.VerifyNoOtherCalls();
        fixture.MocksDict.First(d => d.Key.Target == obj).Value?.Result?.GetValidValues().ForEach(m => m.VerifyNoOtherCalls());
    }

    public static void VerifyNoOtherCalls(this IAutoMockFixture fixture)
    {
        fixture.TrackerDict.Keys.ToList().ForEach(k => fixture.VerifyNoOtherCalls(k));
    }

    #endregion
}
