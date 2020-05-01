using Interfaces.Delegates.GetFilename;

namespace Delegates.GetFilename
{
    public abstract class GetFixedFilenameDelegate : IGetFilenameDelegate
    {
        private readonly string fixedFilename;

        public GetFixedFilenameDelegate(string fixedFilename, IGetFilenameDelegate getFilenameDelegate)
        {
            this.fixedFilename = getFilenameDelegate.GetFilename(fixedFilename);
        }

        public string GetFilename(string source = null)
        {
            return fixedFilename;
        }
    }
}