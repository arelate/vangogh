using Interfaces.Destination.Filename;

namespace Controllers.Destination.Filename
{
    public class FixedFilenameDelegate : IGetFilenameDelegate
    {
        private string fixedFilename;

        public FixedFilenameDelegate(string fixedFilename, IGetFilenameDelegate jsonFilenameDelegate)
        {
            this.fixedFilename = jsonFilenameDelegate.GetFilename(fixedFilename);
        }

        public string GetFilename(string source = null)
        {
            return fixedFilename;
        }
    }
}
