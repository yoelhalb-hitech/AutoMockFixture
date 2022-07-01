using AutoFixture.Kernel;
using System;
using System.Collections.Generic;
using System.Text;

namespace AutoMoqExtensions.FixtureUtils
{    
    public class FreezeOnRecursionHandler : IRecursionHandler
    {  
      
        public object HandleRecursiveRequest(
            object request,
            IEnumerable<object> recordedRequests)
        {
            return request;
        }
    }
}
