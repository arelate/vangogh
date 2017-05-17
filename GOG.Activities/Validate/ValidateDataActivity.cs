using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Validation;
using Interfaces.Status;

namespace GOG.Activities.Validate
{
    public class ValidateDataActivity: Activity
    {
        private IValidateFileAsyncDelegate<bool> dataFileValidateDelegate;

        public ValidateDataActivity(
            IValidateFileAsyncDelegate<bool> dataFileValidateDelegate,
            IStatusController statusController) :
            base(statusController)
        {
            this.dataFileValidateDelegate = dataFileValidateDelegate;
        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            return base.ProcessActivityAsync(status);
        }
    }
}
