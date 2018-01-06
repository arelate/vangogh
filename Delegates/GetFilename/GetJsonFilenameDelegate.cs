using Interfaces.Delegates.GetFilename;

namespace Delegates.GetFilename
{
    public class GetJsonFilenameDelegate : IGetFilenameDelegate
    {
        private string extension = ".json";

        public string GetFilename(string source = null)
        {
            return source + extension;
        }
    }
}
