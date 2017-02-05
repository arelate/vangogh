using Interfaces.Destination;

namespace Controllers.Destination
{
    public class FixedFilenameDelegate : IGetFilenameDelegate
    {
        private string extension = ".json";
        private string filename;

        public FixedFilenameDelegate(string filename)
        {
            this.filename = filename;
        }

        public string GetFilename(string source = null)
        {
            return filename + extension;
        }
    }
}
