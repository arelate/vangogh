namespace Controllers.Destination.Data
{
    public class ScheduledValidationsDestinationController: DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "scheduledValidations.js";
        }
    }
}
