using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.TaskStatus
{
    public interface ICreateViewDelegate
    {
        void CreateView(bool overrideThrottling = false);
    }

    public interface ITaskStatusViewController:
        ICreateViewDelegate
    {
        // ...
    }
}
