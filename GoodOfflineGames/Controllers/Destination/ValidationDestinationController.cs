using System.IO;

using Interfaces.Destination;

namespace Controllers.Destination
{
    public class ValidationDestinationController : IDestinationController
    {
        private IDestinationController gogUriDestinationController;

        public ValidationDestinationController(IDestinationController gogUriDestinationController)
        {
            this.gogUriDestinationController = gogUriDestinationController;
        }

        public string GetDirectory(string source)
        {
            return "_md5";
        }

        public string GetFilename(string source)
        {
            return gogUriDestinationController.GetFilename(source);
        }
    }
}
