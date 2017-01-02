namespace Controllers.Destination.Data
{
    public class ScheduledCleanupDestinationController : DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "scheduledCleanup.js";
        }
    }
}
