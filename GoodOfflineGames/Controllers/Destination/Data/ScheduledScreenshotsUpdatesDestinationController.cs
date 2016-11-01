namespace Controllers.Destination.Data
{
    public class ScheduledScreenshotsUpdatesDestinationController : DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "scheduledScreenshotsUpdates.js";
        }
    }
}
