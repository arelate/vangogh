using System.Threading.Tasks;

namespace Interfaces.Session
{
    public interface ISessionProperty
    {
        string Session { get; }
    }

    public interface IGetUriSansSessionDelegate
    {
        string GetUriSansSession(string sessionUri);
    }

    public interface ICreateSessionDelegate
    {
        Task CreateSession(string manualUri);
    }

    public interface ISessionController :
        ISessionProperty,
        ICreateSessionDelegate,
        IGetUriSansSessionDelegate
    {
        // ...
    }
}
