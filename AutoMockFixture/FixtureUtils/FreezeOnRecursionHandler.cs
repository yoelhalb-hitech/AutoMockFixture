
namespace AutoMoqExtensions.FixtureUtils;

public class FreezeOnRecursionHandler : IRecursionHandler
{      
    public object HandleRecursiveRequest(
        object request,
        IEnumerable<object> recordedRequests)
    {
        return request;
    }
}
