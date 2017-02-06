using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class JsonFilenameDelegate : IGetFilenameDelegate
    {
        private string extension = ".json";

        public string GetFilename(string source = null)
        {
            return source + extension;
        }
    }
}
