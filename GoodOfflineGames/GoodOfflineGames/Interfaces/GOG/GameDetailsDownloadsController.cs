using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface IExtractSingleDelegate
    {
        string ExtractSingle(string input);
    }

    public interface ISanitazeDelegate
    {
        string Sanitize(string input1, string input2);
    }

    public interface IGameDetailsDownloadsController:
        IExtractSingleDelegate,
        ISanitazeDelegate
    {
        // ...
    }
}
