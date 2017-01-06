using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Console;
using Interfaces.TaskStatus;

namespace Controllers.TaskStatus
{
    public class TaskStatusViewController: ITaskStatusViewController
    {
        private ITaskStatus applicationTaskStatus;
        private IConsoleController consoleController;

        public TaskStatusViewController(
            ITaskStatus applicationTaskStatus,
            IConsoleController consoleController)
        {
            this.applicationTaskStatus = applicationTaskStatus;
            this.consoleController = consoleController;
        }

        public void CreateView()
        {
            //throw new NotImplementedException();
        }
    }
}
