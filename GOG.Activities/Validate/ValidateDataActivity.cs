using System.Linq;
using System.Threading.Tasks;

using Interfaces.Controllers.File;
using Interfaces.Controllers.Hash;

using Interfaces.Validation;
using Interfaces.Status;

namespace GOG.Activities.Validate
{
    public class ValidateDataActivity: Activity
    {
        readonly IStoredHashController storedHashController;
        readonly IFileController fileController;
        readonly IValidateFileAsyncDelegate<bool> fileValidateDelegate;

        public ValidateDataActivity(
            IStoredHashController storedHashController,
            IFileController fileController,
            IValidateFileAsyncDelegate<bool> fileValidateDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.storedHashController = storedHashController;
            this.fileController = fileController;
            this.fileValidateDelegate = fileValidateDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var validateDataTask = await statusController.CreateAsync(status, "Validate data");

            var dataFiles = await storedHashController.ItemizeAllAsync(validateDataTask);
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

                var precomputedHash = await storedHashController.GetHashAsync(dataFile, validateDataTask);
                if(!await fileValidateDelegate.ValidateFileAsync(dataFile, precomputedHash, validateDataTask))
                    await statusController.WarnAsync(validateDataTask, $"Data file {dataFile} hash doesn't match precomputed value");
            }

            await statusController.CompleteAsync(validateDataTask);
        }
    }
}
