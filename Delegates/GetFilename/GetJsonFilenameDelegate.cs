using Interfaces.Delegates.GetFilename;

namespace Delegates.GetFilename
{
    public class GetJsonFilenameDelegate : IGetFilenameDelegate
    {
        string extension = ".json";

        public string GetFilename(string source = null)
        {
            return source + extension;
        }
    }
}
