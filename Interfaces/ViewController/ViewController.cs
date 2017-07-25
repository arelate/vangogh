using System.Threading.Tasks;

namespace Interfaces.ViewController
{
    public interface IRequestUpdatedViewDelegate<T>
    {
        T RequestUpdatedView();
    }

    public interface IPostUpdateNotificationDelegate
    {
        void PostUpdateNotification();
    }

    public interface IPostUpdateNotificationAsyncDelegate
    {
        Task PostUpdateNotificationAsync();
    }

    public interface IViewController<T>:
        IRequestUpdatedViewDelegate<T>,
        IPostUpdateNotificationDelegate,
        IPostUpdateNotificationAsyncDelegate
    {
        // ...
    }
}
