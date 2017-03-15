using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ViewController
{
    public interface ICreateViewDelegate
    {
        void CreateView(bool overrideThrottling = false);
    }

    public interface IViewController:
        ICreateViewDelegate
    {
        // ...
    }
}
