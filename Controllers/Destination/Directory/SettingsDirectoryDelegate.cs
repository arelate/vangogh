using Interfaces.Destination.Directory;

namespace Controllers.Destination.Directory
{
    public class SettingsDirectoryDelegate : IGetDirectoryDelegate
    {
        private IGetDirectoryDelegate settingsController;
        private string directory;

        public SettingsDirectoryDelegate(
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
