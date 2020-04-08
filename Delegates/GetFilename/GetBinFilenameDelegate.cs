using Interfaces.Delegates.GetFilename;
using Models.Extensions;

namespace Delegates.GetFilename
{
    public class GetBinFilenameDelegate : IGetFilenameDelegate
    {
        public string GetFilename(string source = null)
        {
            return source + Extensions.BIN;
        }
    }
}