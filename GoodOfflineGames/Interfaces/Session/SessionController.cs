using System.Threading.Tasks;

namespace Interfaces.Session
{
    public interface IGetUriSansSessionDelegate
    {
        string GetUriSansSession(string sessionUri);
    }

    public interface ICreateSessionDelegate
    {
        Task<string> CreateSession(string manualUri);
    }

    public interface ISessionController :
        ICreateSessionDelegate,
        IGetUriSansSessionDelegate
    {
        // ...
    }
}
