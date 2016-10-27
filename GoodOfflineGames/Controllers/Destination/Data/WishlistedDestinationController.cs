namespace Controllers.Destination.Data
{
    public class WishlistedDestinationController : DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "wishlisted.js";
        }
    }
}
