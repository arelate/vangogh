using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Status;

using Interfaces.Controllers.Data;
using Interfaces.ValidationResults;

using Models.ValidationResults;

namespace GOG.Activities.Repair
{
    public class RepairActivity : Activity
    {
        readonly IDataController<ValidationResults> validationResultDataController;
        readonly IValidationResultController validationResultController;

        public RepairActivity(
            IDataController<ValidationResults> validationResultDataController,
            IValidationResultController validationResultController,
            IStatusController statusController) : base(statusController)
        {
            this.validationResultDataController = validationResultDataController;
            this.validationResultController = validationResultController;
        }

        public override async Task ProcessActivityAsync(IStatus status)
        {
            var repairTask = await statusController.CreateAsync(status, "Check validation results and attempting repair");
            var current = 0;

            var invalidResults = new List<ValidationResults>();

            foreach (var id in await validationResultDataController.ItemizeAllAsync(repairTask))
            {
                var validationResult = await validationResultDataController.GetByIdAsync(id, repairTask);

                if (validationResult == null) continue;

                await statusController.UpdateProgressAsync(
                    repairTask,
                    ++current,
                    await validationResultDataController.CountAsync(repairTask),
                    validationResult.Title);

                if (!validationResultController.ProductIsValid(validationResult))
                    invalidResults.Add(validationResult);

            }

            await statusController.CompleteAsync(repairTask);
        }
    }
}
