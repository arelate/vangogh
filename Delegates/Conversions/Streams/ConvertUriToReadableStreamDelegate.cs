using System.IO;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Streams
{
    public class ConvertUriToReadableStreamDelegate : IConvertDelegate<string, Stream>
    {
        public Stream Convert(string uri)
        {
            return new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        }
    }
}