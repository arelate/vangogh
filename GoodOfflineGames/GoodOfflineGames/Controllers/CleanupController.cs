using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;
using GOG.Model;

namespace GOG.Controllers
{
    // TODO: Unit tests
    public class CleanupController : ICleanupController
    {
        private IIOController ioController;
        private IProductFileController productFileController;

        public CleanupController(
            IProductFileController productFileController, 
            IIOController ioController)
        {
            this.ioController = ioController;
            this.productFileController = productFileController;
        }

        public int Cleanup(
            string removeToFolder, 
            IPostUpdateDelegate postUpdateDelegate = null)
        {
            int movedFiles = 0;

            foreach (string folder in productFileController.GetFolders())
            {
                var expectedFiles = productFileController.GetFiles(folder);
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
