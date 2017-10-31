using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Activity;
using Interfaces.Status;

using GOG.Models;

namespace GOG.Activities.List
{
    public class ListUpdatedActivity : Activity
    {
        IDataController<AccountProduct> accountProductsDataController;

        public ListUpdatedActivity(
            IDataController<AccountProduct> accountProductsDataController,
            IStatusController statusController): 
            base(statusController)
        {
            this.accountProductsDataController = accountProductsDataController;
        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            return base.ProcessActivityAsync(status);
        }
    }
}
