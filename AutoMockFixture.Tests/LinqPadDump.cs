using AutoMockFixture.FixtureUtils.Requests;

namespace AutoMockFixture.Tests;

public static class LinqPadDump
{
    public static object ToDump(this AutoMockFixture.FixtureUtils.AutoMockFixtureBase fixture, object obj)
    {
        var tracker = fixture.GetTracker(obj);

        if (tracker is null) return "N/A";

        return ToDump(tracker);
    }

    private static object ToDump(ITracker tracker)
    {
        return new
        {
            Request = tracker.GetType().Name,
            Type = (tracker as IRequestWithType)?.Request.Name,
            tracker.IsCompleted,
            tracker.InstancePath,
            Builder = tracker.Builder?.GetType().Name,
            Command = tracker.Command?.GetType().Name,
            Result = tracker.Result is null ? null : new Lazy<object?>(() => tracker.Result),
            Children = tracker.Children?.Select(c => ToDump(c)).ToList(),
        };
    }
}
