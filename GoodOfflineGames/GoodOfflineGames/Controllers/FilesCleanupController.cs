using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public class FilesCleanupController
    {
        private IIOController ioController;

        public FilesCleanupController(IIOController ioController)
        {
            this.ioController = ioController;
        }

        public void Cleanup(IDictionary<string, IList<string>> filesInFolders, string removeToFolder)
        {
            foreach (string folder in filesInFolders.Keys)
            {
                var expectedFiles = filesInFolders[folder];
                foreach (string file in ioController.EnumerateFiles(folder))
                {
                    var existingFile = Path.GetFileName(file);
                    if (expectedFiles.Contains(existingFile)) continue;
                    Console.WriteLine("{0} in {1}", existingFile, folder);
                }
            }
        }
    }
}
