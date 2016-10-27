namespace Controllers.Destination.Data
{
    public class UpdatedDestinationController : DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "updated.js";
        }
    }
}
