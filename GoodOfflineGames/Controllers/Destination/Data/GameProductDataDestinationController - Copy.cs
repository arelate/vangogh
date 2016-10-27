using System.IO;

namespace Controllers.Destination.Data
{
    public class GameProductDataDestinationController : DataDestinationController
    {
        public override string GetDirectory(string source)
        {
            return Path.Combine(
                base.GetDirectory(source),
                "gameProductData");
        }
    }
}
