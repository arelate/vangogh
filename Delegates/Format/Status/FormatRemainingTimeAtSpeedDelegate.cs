using System;

using Interfaces.Delegates.Format;

using Interfaces.Status;

namespace Delegates.Format.Status
{
    public class FormatRemainingTimeAtSpeedDelegate : IFormatDelegate<IStatus, Tuple<long, double>>
    {
        public Tuple<long, double> Format(IStatus status)
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
