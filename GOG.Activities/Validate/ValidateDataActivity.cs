using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Interfaces.Status;

namespace GOG.Activities.Validate
{
    public class ValidateDataActivity: Activity
    {
        public ValidateDataActivity(
            IStatusController statusController) :
            base(statusController)
        {

        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            return base.ProcessActivityAsync(status);
        }
    }
}
