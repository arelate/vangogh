using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class FixedJsonFilenameDelegate : IGetFilenameDelegate
    {
        private string extension = ".json";
        private string fixedFilename;

        public FixedJsonFilenameDelegate(string fixedFilename)
        {
            this.fixedFilename = fixedFilename;
        }

        public string GetFilename(string source = null)
        {
            return string.IsNullOrEmpty(fixedFilename) ?
                source + extension :
                fixedFilename + extension;
        }
    }
}
