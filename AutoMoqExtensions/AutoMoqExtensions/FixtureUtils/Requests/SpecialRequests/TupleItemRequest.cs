using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils.Requests
{
    // TODO... now assumming that we won't overload by tuple (not tuple lentgh and not tuple item types)
    internal class TupleItemRequest : OneOfMultipleRequest
    {
        public TupleItemRequest(Type request, int index, bool? autoMock, ITracker? tracker) : base(request, index, autoMock, tracker)
        {
        }

        public override string InstancePath => $"({"".PadLeft(Index,',')})";

        public override bool IsRequestEquals(ITracker other) => other is TupleItemRequest && base.IsRequestEquals(other);
    }
}
