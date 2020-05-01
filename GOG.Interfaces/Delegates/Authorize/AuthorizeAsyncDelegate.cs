using System.Threading.Tasks;

namespace GOG.Interfaces.Delegates.Authorize
{
    public interface IAuthorizeAsyncDelegate
    {
        Task AuthorizeAsync(string username, string password);
    }

}