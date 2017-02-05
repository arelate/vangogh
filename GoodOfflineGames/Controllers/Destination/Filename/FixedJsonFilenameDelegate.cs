using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class FixedJsonFilenameDelegate : IGetFilenameDelegate
    {
        private string extension = ".json";
        private string filename;

        public FixedJsonFilenameDelegate(string filename)
        {
            this.filename = filename;
        }

        public string GetFilename(string source = null)
        {
            return filename + extension;
        }
    }
}
