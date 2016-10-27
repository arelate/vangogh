namespace Controllers.Destination.Data
{
    public class ProductsDestinationController : DataDestinationController
    {
        public override string GetFilename(string source)
        {
            return "products.js";
        }
    }
}
