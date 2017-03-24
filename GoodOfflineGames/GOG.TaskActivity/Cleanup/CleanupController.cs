using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interfaces.TaskStatus;

using Interfaces.Enumeration;
using Interfaces.RecycleBin;

namespace GOG.TaskActivities.Cleanup
{
    public class CleanupController : TaskActivityController
    {
        private IEnumerateAsyncDelegate expectedItemsEnumarateDelegate;
        private IEnumerateDelegate actualItemsEnumerateDelegate;
        private IEnumerateDelegate<string> itemsDetailsEnumerateDelegate;
        private IRecycleBinController recycleBinController;

        public CleanupController(
            IEnumerateAsyncDelegate expectedItemsEnumarateDelegate,
            IEnumerateDelegate actualItemsEnumerateDelegate,
            IEnumerateDelegate<string> itemsDetailsEnumerateDelegate,
            IRecycleBinController recycleBinController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.expectedItemsEnumarateDelegate = expectedItemsEnumarateDelegate;
            this.actualItemsEnumerateDelegate = actualItemsEnumerateDelegate;
            this.itemsDetailsEnumerateDelegate = itemsDetailsEnumerateDelegate;
            this.recycleBinController = recycleBinController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            var expectedItems = await expectedItemsEnumarateDelegate.EnumerateAsync(taskStatus);
            var actualItems = actualItemsEnumerateDelegate.Enumerate(taskStatus);

            var unexpectedItems = actualItems.Except(expectedItems);
            var detailedUnexpectedItems = new List<string>();

            foreach (var unexpectedItem in unexpectedItems)
                detailedUnexpectedItems.AddRange(itemsDetailsEnumerateDelegate.Enumerate(unexpectedItem));

            var moveToRecycleBinTask = taskStatusController.Create(taskStatus, "Move unexpected items to recycle bin");

            foreach (var item in detailedUnexpectedItems)
                recycleBinController.MoveToRecycleBin(item);

            taskStatusController.Complete(moveToRecycleBinTask);
        }
    }
}
