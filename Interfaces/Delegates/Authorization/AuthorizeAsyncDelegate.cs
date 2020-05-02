using System.Threading.Tasks;

namespace Interfaces.Delegates.Authorization
{
    public interface IAuthorizeAsyncDelegate
    {
        Task AuthorizeAsync(string username, string password);
    }

}