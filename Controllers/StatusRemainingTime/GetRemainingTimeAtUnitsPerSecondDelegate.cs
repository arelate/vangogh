using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Status;
using Interfaces.StatusProgress;

namespace Controllers.StatusRemainingTime
{
    public class GetRemainingTimeAtUnitsPerSecondDelegate : IGetRemainingTimeAtUnitsPerSecondDelegate
    {
        public Tuple<long, double> GetRemainingTimeAtUnitsPerSecond(IStatus status)
        {
            if (status == null ||
                status.Progress == null) return new Tuple<long, double>(0, 0);

            var total = status.Progress.Total;
            var current = status.Progress.Current;

            var elapsed = DateTime.UtcNow - status.Started;
            var unitsPerSecond = status.Progress.Current / elapsed.TotalSeconds;
            var remainingTime = ((long)((total - current) / unitsPerSecond));

            return new Tuple<long, double>(remainingTime, unitsPerSecond);
        }
    }
}
