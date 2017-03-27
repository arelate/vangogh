using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.TaskStatus;

namespace Interfaces.RequestPage
{
    public interface IRequestPageDelegate
    {
        Task<string> RequestPage(string uri, IDictionary<string, string> parameters, int page, ITaskStatus taskStatus);
    }

    public interface IRequestPageController:
        IRequestPageDelegate
    {
        // ...
    }
}
