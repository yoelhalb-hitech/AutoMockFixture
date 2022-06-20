using AutoFixture.Kernel;
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

        public void AddIfNeeded(object request, object? specimen)
        {
            if (!CacheSpecifications.Any(s => s.IsSatisfiedBy(request))) return;

            if (CacheDictionary.ContainsKey(request)
                    && CacheDictionary[request] == specimen) return;

            if (CacheDictionary.ContainsKey(request)
                    && CacheDictionary[request] != specimen) throw new Exception("A different object is already in cache");

            CacheDictionary[request] = specimen;
        }
    }
}
