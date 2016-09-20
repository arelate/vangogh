using Interfaces.Destination;

namespace Controllers.Destination
{
    public class FilesExtrasDestinationController : IDestinationController
    {
        IDestinationController gogUriDestinationController;

        public FilesExtrasDestinationController(IDestinationController gogUriDestinationController)
        {
            this.gogUriDestinationController = gogUriDestinationController;
        }

        public string GetDirectory(string source)
        {
            return gogUriDestinationController.GetDirectory(source);
        }

        public string GetFilename(string source)
        {
            return gogUriDestinationController.GetFilename(source);
        }
    }
}
