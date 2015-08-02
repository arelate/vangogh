using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface IAuthenticationController
    {
        Task<bool> Authorize(ICredentials credentials);
    }
}
