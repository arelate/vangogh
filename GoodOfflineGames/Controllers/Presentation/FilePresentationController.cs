using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Presentation;

namespace Controllers.Presentation
{
    public class FilePresentationController : IPresentationController<string>
    {
        public void Present(IEnumerable<string> views)
        {
            throw new NotImplementedException();
        }

        public Task PresentAsync(IEnumerable<string> views)
        {
            throw new NotImplementedException();
        }
    }
}
