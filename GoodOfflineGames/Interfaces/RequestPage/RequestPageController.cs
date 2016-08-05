using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.RequestPage
{
    public interface IRequestPageDelegate
    {
        Task<string> RequestPage(string uri, IDictionary<string, string> parameters, int page);
    }

    public interface IRequestPageController:
        IRequestPageDelegate
    {
        // ...
    }
}
