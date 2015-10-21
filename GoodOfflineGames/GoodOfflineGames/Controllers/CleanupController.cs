using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public class CleanupController : ICleanupController
    {
        private IIOController ioController;

        public CleanupController(IIOController ioController)
        {
            this.ioController = ioController;
        }

        public int Cleanup(
            IDictionary<string, IList<string>> filesInFolders, 
            string removeToFolder, 
            IPostUpdateDelegate postUpdateDelegate = null)
        {
            int movedFiles = 0;

            foreach (string folder in filesInFolders.Keys)
            {
                var expectedFiles = filesInFolders[folder];
                foreach (string file in ioController.EnumerateFiles(folder))
                {
                    var existingFile = Path.GetFileName(file);
                    if (expectedFiles.Contains(existingFile)) continue;

                    var to = Path.Combine(removeToFolder, file);
                    var toFolder = Path.GetDirectoryName(to);

                    if (!ioController.DirectoryExists(toFolder))
                        ioController.CreateDirectory(toFolder);

                    ioController.MoveFile(file, to);
                    movedFiles++;

                    if (postUpdateDelegate != null)
                    {
                        postUpdateDelegate.PostUpdate();
                    }
                }
            }

            return movedFiles;
        }
    }
}
