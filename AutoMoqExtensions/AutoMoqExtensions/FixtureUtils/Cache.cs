using AutoFixture.Kernel;
using AutoMoqExtensions.FixtureUtils.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.FixtureUtils
{
    internal class Cache
    {
        public List<IRequestSpecification> CacheSpecifications { get; } = new List<IRequestSpecification>();
        public Dictionary<object, object?> CacheDictionary { get; } = new Dictionary<object, object?>();

        public (bool HasValue, object? Value) Get(object request)
        {
            if (CacheDictionary.ContainsKey(request)) return (true, CacheDictionary[request]);

            if (request is ITracker tracker && CacheDictionary.Any(c => (c.Key as ITracker)?.IsRequestEquals(tracker) == true))
            {               
                var existing = CacheDictionary.First(c => (c.Key as ITracker)?.IsRequestEquals(tracker) == true);
                return (true, existing.Value);
            }

            return (false, null);
        }

        public void AddIfNeeded(object request, object? specimen)
        {
            if (!CacheSpecifications.Any(s => s.IsSatisfiedBy(request))) return;

            var existing = Get(request);
            if(existing.HasValue && existing.Value == specimen) return;
            
            if(existing.HasValue) throw new Exception("A different object is already in cache");            
            
            CacheDictionary[request] = specimen;
        }
    }
}
