using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Delegates.Trace;

namespace Delegates.Trace
{
    public class IOTraceDelegate: ITraceDelegate
    {
        readonly IList<string> ioOperations;

        public IOTraceDelegate(IList<string> ioOperations)
        {
            this.ioOperations = ioOperations;
        }

        public void Trace(params string[] parameters)
        {
            ioOperations.Add(string.Join(",", parameters));
        }
    }
}
