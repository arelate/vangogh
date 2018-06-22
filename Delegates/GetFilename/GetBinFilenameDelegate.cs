using Interfaces.Delegates.GetFilename;

namespace Delegates.GetFilename
{
    public class GetBinFilenameDelegate : IGetFilenameDelegate
    {
        const string extension = ".bin";

        public string GetFilename(string source = null)
        {
            return source + extension;
        }
    }
}
