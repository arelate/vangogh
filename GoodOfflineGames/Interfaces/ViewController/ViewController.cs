using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.ViewController
{
    public interface IPresentViewsDelegate
    {
        void PresentViews();
    }

    public interface IViewController:
        IPresentViewsDelegate
    {
        // ...
    }
}
