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
        private IFileValidationController fileValidationController;

        public CleanupController(
            IProductFileController productFileController,
            IFileValidationController fileValidationController,
            IIOController ioController)
        {
            this.ioController = ioController;
            this.productFileController = productFileController;
            this.fileValidationController = fileValidationController;
        }

        public bool CleanupValidationFile(string localFile, string removeToFolder)
        {
            var localValidationFile = fileValidationController.GetLocalValidationFilename(localFile);
            if (ioController.FileExists(localValidationFile))
            {
                var to = Path.Combine(removeToFolder, Path.GetFileName(localValidationFile));

                ioController.MoveFile(localValidationFile, to);

                return true;
            }

            return false;
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

                    // also cleanup validation file
                    if (CleanupValidationFile(file, removeToFolder)) movedFiles++;

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
