using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class PresentationControllerEx : IPresentationController<string>
    {
        public void Present(IEnumerable<string> views, bool overrideThrottling = false)
        {
            throw new NotImplementedException();
        }
    }
}
