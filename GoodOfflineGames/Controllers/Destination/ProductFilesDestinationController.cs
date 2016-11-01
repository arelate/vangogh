using System.IO;

using Interfaces.Destination;

namespace Controllers.Destination
{
    public class ProductFilesDestinationController : IDestinationController
    {
        private IDestinationController gogUriDestinationController;
        private string productFilesDestination;

        public ProductFilesDestinationController(
            string productFilesDestination,
            IDestinationController gogUriDestinationController)
        {
            this.productFilesDestination = productFilesDestination;
            this.gogUriDestinationController = gogUriDestinationController;
        }

        public string GetDirectory(string source)
        {
            return Path.Combine(
                productFilesDestination, 
                gogUriDestinationController.GetDirectory(source));
        }

        public string GetFilename(string source)
        {
            return gogUriDestinationController.GetFilename(source);
        }
    }
}
