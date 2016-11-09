using System;
using System.Threading.Tasks;

using Interfaces.Reporting;
using Interfaces.Validation;
using Interfaces.Destination;
using Interfaces.Data;

using GOG.TaskActivities.Abstract;

using GOG.Models.Custom;

namespace GOG.TaskActivities.Validation
{
    public class ProcessValidationController : TaskActivityController
    {
        private IDestinationController destinationController;
        private IValidationController validationController;
        private IDataController<long> updatedDataController;
        private IDataController<long> lastKnownValidDataController;
        private IDataController<ScheduledValidation> scheduledValidationDataController;

        public ProcessValidationController(
            IDestinationController destinationController,
            IValidationController validationController,
            IDataController<long> updatedDataController,
            IDataController<long> lastKnownValidDataController,
            IDataController<ScheduledValidation> scheduledValidationDataController,
            ITaskReportingController taskReportingController) :
            base(taskReportingController)
        {
            this.destinationController = destinationController;
            this.validationController = validationController;

            this.updatedDataController = updatedDataController;
            this.scheduledValidationDataController = scheduledValidationDataController;
            this.lastKnownValidDataController = lastKnownValidDataController;
        }

        public override async Task ProcessTask()
        {
            taskReportingController.StartTask("Validate product files");

            foreach (var id in scheduledValidationDataController.EnumerateIds())
            {
                var productIsValid = true;

                var scheduledValidation = await scheduledValidationDataController.GetById(id);

                foreach (var file in scheduledValidation.Files)
                {
                    try
                    {
                        taskReportingController.StartTask("Validate product file: {0}, {1}", scheduledValidation.Title, file);
                        await validationController.Validate(file);
                        productIsValid &= true;
                    }
                    catch (Exception ex)
                    {
                        taskReportingController.ReportFailure(ex.Message);
                        productIsValid &= false;
                    }
                    finally
                    {
                        taskReportingController.CompleteTask();
                    }
                }

                if (productIsValid)
                {
                    taskReportingController.StartTask("Congratulations, all product files are valid! removing product from updates: {0}", scheduledValidation.Title);
                    await lastKnownValidDataController.Update(id);
                    await updatedDataController.Remove(id);
                    taskReportingController.CompleteTask();
                }
                else
                {
                    taskReportingController.StartTask("Unfortunately, some product files failed validation, updating last known valid state: {0}", scheduledValidation.Title);
                    await lastKnownValidDataController.Remove(id);
                    taskReportingController.CompleteTask();
                }
            }

            taskReportingController.CompleteTask();
        }
    }
}
