using System.Threading.Tasks;

using Interfaces.Status;

namespace Interfaces.Delegates.GetDirectory
{
    public interface IGetDirectoryDelegate
    {
        string GetDirectory(string input);
    }
}
