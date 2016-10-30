namespace Controllers.Destination.Data
{
    public class ScheduledDownloadsDestinationController: DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "scheduledDownloads.js";
        }
    }
}
