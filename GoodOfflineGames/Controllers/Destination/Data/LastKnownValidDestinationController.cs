namespace Controllers.Destination.Data
{
    public class LastKnownValidDestinationController: DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "lastKnownValid.js";
        }
    }
}
