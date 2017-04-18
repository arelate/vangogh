using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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
            if (source == null) source = string.Empty;

            return Path.Combine(
                settingsController.GetDirectory(directory),
                source);
        }
    }
}
