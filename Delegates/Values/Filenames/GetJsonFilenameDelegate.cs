using Interfaces.Delegates.Values;
using Models.Extensions;

namespace Delegates.Values.Filenames
{
    public class GetJsonFilenameDelegate : IGetValueDelegate<string, string>
    {
        public string GetValue(string source = null)
        {
            return source + Extensions.JSON;
        }
    }
}