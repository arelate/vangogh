using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.TaskStatus;
using Interfaces.Template;

namespace GOG.TaskActivities.Template
{
    public class TestTemplateController : TaskActivityController
    {
        private ITemplateController templateController;

        public TestTemplateController(
            ITemplateController templateController,
            ITaskStatusController taskStatusController) :
            base(taskStatusController)
        {
            this.templateController = templateController;
        }

        public override async Task ProcessTaskAsync(ITaskStatus taskStatus)
        {
            //templateController.Bind("taskStatus", new Dictionary<string, string>()
            //{
            //    {"title", "Welcome to GoodOfflineGames"},
            //    {"containsProgress", "true"},
            //    {"progressPercent", "90%"},
            //    { "progressCurrent", "9" },
            //    { "progressTotal", "10" },
            //    {"containsEta", "true" },
            //    {"remainingTime", "1 day 2 hours and 5 minutes"},
            //    { "averageUnitsPerSecond", "speed of light" }
            //});

            //return base.ProcessTaskAsync(taskStatus);
        }

    }
}
