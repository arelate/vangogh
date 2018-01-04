using System;

using Interfaces.Status;

namespace Interfaces.StatusRemainingTime
{
    public interface IGetRemainingTimeAtUnitsPerSecondDelegate
    {
        Tuple<long, double> GetRemainingTimeAtUnitsPerSecond(IStatus status);
    }
}
