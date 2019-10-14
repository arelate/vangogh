using System.Linq;
using System.Threading.Tasks;

using Interfaces.Controllers.File;
using Interfaces.Controllers.Hashes;

using Interfaces.Validation;
using Interfaces.Status;

namespace GOG.Activities.Validate
{
    public class ValidateDataActivity: Activity
    {
        readonly IHashesController hashesController;
        readonly IFileController fileController;
        readonly IValidateFileAsyncDelegate<bool> validateFileDelegate;

        public ValidateDataActivity(
            IHashesController hashesController,
            IFileController fileController,
            IValidateFileAsyncDelegate<bool> validateFileDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.hashesController = hashesController;
            this.fileController = fileController;
            this.validateFileDelegate = validateFileDelegate;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var validateDataTask = await statusController.CreateAsync(status, "Validate data");

            var dataFiles = await hashesController.ItemizeAllAsync(validateDataTask);
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

                var precomputedHash = await hashesController.ConvertAsync(dataFile, validateDataTask);
                if(!await validateFileDelegate.ValidateFileAsync(dataFile, precomputedHash, validateDataTask))
                    await statusController.WarnAsync(validateDataTask, $"Data file {dataFile} hash doesn't match precomputed value");
            }

            await statusController.CompleteAsync(validateDataTask);
        }
    }
}
