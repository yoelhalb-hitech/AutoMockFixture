using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.Extensions
{
    internal static class TrackerExtensions
    {
        public static IEnumerable<ITracker> GetParentsOnCurrentLevel(this ITracker tracker)
        {
            var currentLevelPath = tracker.Path;
            var current = tracker;

            while(current.Parent is not null
                    && !object.ReferenceEquals(current.Parent, current))
            {
                current = current.Parent;

                if (current.Path != currentLevelPath) yield break;

                yield return current;
            }
        }
    }
}
