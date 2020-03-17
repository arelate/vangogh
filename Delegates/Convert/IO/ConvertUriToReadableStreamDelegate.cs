using System.IO;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert.IO
{
    public class ConvertUriToReadableStreamDelegate : IConvertDelegate<string, Stream>
    {
        public Stream Convert(string uri)
        {
            return new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}