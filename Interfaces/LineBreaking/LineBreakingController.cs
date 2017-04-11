using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.LineBreaking
{
    public interface IBreakLinesDelegate
    {
        string[] BreakLines(string content, int availableWidth);
    }
    public interface ILineBreakingController:
        IBreakLinesDelegate
    {
        // ...
    }
}
