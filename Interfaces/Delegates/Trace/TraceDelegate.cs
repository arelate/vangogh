using System;

namespace Interfaces.Delegates.Trace
{
    public interface ITraceDelegate
    {
        void Trace(params string[] parameters);
    }
}
