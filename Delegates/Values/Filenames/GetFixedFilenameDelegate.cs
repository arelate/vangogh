using Interfaces.Delegates.Values;

namespace Delegates.Values.Filenames
{
    public abstract class GetFixedFilenameDelegate : IGetValueDelegate<string, string>
    {
        private readonly string fixedFilename;

        public GetFixedFilenameDelegate(string fixedFilename, IGetValueDelegate<string, string> getFilenameDelegate)
        {
            this.fixedFilename = getFilenameDelegate.GetValue(fixedFilename);
        }

        public string GetValue(string source = null)
        {
            return fixedFilename;
        }
    }
}