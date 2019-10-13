using Interfaces.Delegates.GetFilename;

using Models.Extensions;

namespace Delegates.GetFilename
{
    public class GetJsonFilenameDelegate : IGetFilenameDelegate
    {
        public string GetFilename(string source = null)
        {
            return source + Extensions.JSON;
        }
    }
}
