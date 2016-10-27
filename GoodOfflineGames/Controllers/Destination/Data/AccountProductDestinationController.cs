namespace Controllers.Destination.Data
{
    public class AccountProductDestinationController: DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "accountProducts.js";
        }
    }
}
