using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMoqExtensions.MockUtils
{
    public class CannotSetupMethodException
    {
        public CannotSetupReason Reason { get; }
        public Exception? Exception { get; }

        public enum CannotSetupReason
        {
            NonVirtual,
            Private,
            Protected,
            CallBaseNoAbstract,
            Exception
        }

        public CannotSetupMethodException(CannotSetupReason reason, Exception? exception = null)
        {
            Reason = reason;
            Exception = exception;
        }
    }
}
