using Interfaces.Delegates.GetFilename;

namespace Delegates.GetFilename
{
    public class GetFixedFilenameDelegate : IGetFilenameDelegate
    {
        readonly string fixedFilename;

        public GetFixedFilenameDelegate(string fixedFilename, IGetFilenameDelegate jsonFilenameDelegate)
        {
            this.fixedFilename = jsonFilenameDelegate.GetFilename(fixedFilename);
        }

        public string GetFilename(string source = null)
        {
            return fixedFilename;
        }
    }
}
