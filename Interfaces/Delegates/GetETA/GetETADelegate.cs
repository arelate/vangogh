using System;

using Interfaces.Status;

namespace Interfaces.Delegates.GetETA
{
    public interface IGetETADelegate
    {
        Tuple<long, double> GetETA(IStatus status);
    }
}
