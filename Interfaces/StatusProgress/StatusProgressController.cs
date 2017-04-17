using System;

using Interfaces.Status;

namespace Interfaces.StatusProgress
{
    public interface IGetRemainingTimeAtUnitsPerSecondDelegate
    {
        Tuple<long, double> GetRemainingTimeAtUnitsPerSecond(IStatus status);
    }

    public interface IStatusProgressController:
        IGetRemainingTimeAtUnitsPerSecondDelegate
    {
        // ...
    }
}
