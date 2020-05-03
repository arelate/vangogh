using Interfaces.Delegates.Values;
using Models.Extensions;

namespace Delegates.Values.Filenames
{
    public class GetBinFilenameDelegate : IGetValueDelegate<string, string>
    {
        public string GetValue(string source = null)
        {
            return source + Extensions.BIN;
        }
    }
}