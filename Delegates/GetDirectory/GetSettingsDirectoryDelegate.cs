using Interfaces.Delegates.GetDirectory;

namespace Delegates.GetDirectory
{
    public class GetSettingsDirectoryDelegate : IGetDirectoryDelegate
    {
        private IGetDirectoryDelegate settingsController;
        private string directory;

        public GetSettingsDirectoryDelegate(
            string directory,
            IGetDirectoryDelegate settingsController)
        {
            this.directory = directory;
            this.settingsController = settingsController;
        }
        public string GetDirectory(string source = null)
        {
            return settingsController.GetDirectory(directory);
        }
    }
}
