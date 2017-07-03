using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Validation;
using Interfaces.Status;
using Interfaces.Hash;
using Interfaces.File;

namespace GOG.Activities.Validate
{
    public class ValidateDataActivity: Activity
    {
        private IPrecomputedHashController precomputedHashController;
        private IFileController fileController;
        private IValidateFileAsyncDelegate<bool> fileValidateDelegate;

        public ValidateDataActivity(
            IPrecomputedHashController precomputedHashController,
            IFileController fileController,
            IValidateFileAsyncDelegate<bool> fileValidateDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.precomputedHashController = precomputedHashController;
            this.fileController = fileController;
            this.fileValidateDelegate = fileValidateDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var validateDataTask = statusController.Create(status, "Validate data");

            var dataFiles = precomputedHashController.EnumerateKeys();
            var dataFilesCount = dataFiles.Count();
            var current = 0;

            foreach (var dataFile in dataFiles)
            {
                if (!fileController.Exists(dataFile))
                    continue;

                statusController.UpdateProgress(validateDataTask,
                    ++current,
                    dataFilesCount,
                    dataFile);

                var precomputedHash = precomputedHashController.GetHash(dataFile);
                if(!await fileValidateDelegate.ValidateFileAsync(dataFile, precomputedHash, validateDataTask))
                    statusController.Warn(validateDataTask, $"Data file {dataFile} hash doesn't match precomputed value");
            }

            statusController.Complete(validateDataTask);
        }
    }
}
