using System.Threading.Tasks;

namespace Interfaces.ViewController
{
    public interface IPresentViewsDelegate
    {
        void PresentViews();
    }

    public interface IPresentViewsAsyncDelegate
    {
        Task PresentViewsAsync();
    }

    public interface IViewController:
        IPresentViewsDelegate,
        IPresentViewsAsyncDelegate
    {
        // ...
    }
}
