using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Status;

using Interfaces.Data;
using Interfaces.ValidationResult;

using Models.ValidationResult;

namespace GOG.Activities.Repair
{
    public class RepairActivity : Activity
    {
        private IDataController<ValidationResult> validationResultDataController;
        private IValidationResultController validationResultController;

        public RepairActivity(
            IDataController<ValidationResult> validationResultDataController,
            IValidationResultController validationResultController,
            IStatusController statusController) : base(statusController)
        {
            this.validationResultDataController = validationResultDataController;
            this.validationResultController = validationResultController;
        }

        public override async Task ProcessActivityAsync(IStatus status, params string[] parameters)
        {
            var repairTask = statusController.Create(status, "Check validation results and attempting repair");
            var current = 0;

            var invalidResults = new List<ValidationResult>();

            foreach (var id in validationResultDataController.EnumerateIds())
            {
                var validationResult = await validationResultDataController.GetByIdAsync(id);

                if (validationResult == null) continue;

                statusController.UpdateProgress(
                    repairTask,
                    ++current,
                    validationResultDataController.Count(),
                    validationResult.Title);

                if (!validationResultController.ProductIsValid(validationResult))
                    invalidResults.Add(validationResult);

            }

            statusController.Complete(repairTask);
        }
    }
}
