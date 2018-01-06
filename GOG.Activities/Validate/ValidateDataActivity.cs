using System.Linq;
using System.Threading.Tasks;

using Interfaces.Controllers.File;

using Interfaces.Validation;
using Interfaces.Status;
using Interfaces.Hash;

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
            var validateDataTask = await statusController.CreateAsync(status, "Validate data");

            var dataFiles = await precomputedHashController.EnumerateKeysAsync(validateDataTask);
            var dataFilesCount = dataFiles.Count();
            var current = 0;

            foreach (var dataFile in dataFiles)
            {
                if (!fileController.Exists(dataFile))
                    continue;

                await statusController.UpdateProgressAsync(validateDataTask,
                    ++current,
                    dataFilesCount,
                    dataFile);

                var precomputedHash = precomputedHashController.GetHash(dataFile);
                if(!await fileValidateDelegate.ValidateFileAsync(dataFile, precomputedHash, validateDataTask))
                    await statusController.WarnAsync(validateDataTask, $"Data file {dataFile} hash doesn't match precomputed value");
            }

            await statusController.CompleteAsync(validateDataTask);
        }
    }
}
