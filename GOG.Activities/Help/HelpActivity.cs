using System;
using System.Threading.Tasks;

using Interfaces.Status;

namespace GOG.Activities.Help
{
    public class HelpActivity : Activity
    {
        public HelpActivity(
            IStatusController statusController) : 
            base(statusController)
        {
        }

        public override Task ProcessActivityAsync(IStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
