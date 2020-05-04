using System.IO;
using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Streams
{
    public class ConvertUriToWritableStreamDelegate : IConvertDelegate<string, Stream>
    {
        public Stream Convert(string uri)
        {
            var directoryName = Path.GetDirectoryName(uri);
            if (!string.IsNullOrEmpty(directoryName) &&
                !Directory.Exists(directoryName))
                Directory.CreateDirectory(directoryName);

            return new FileStream(uri, FileMode.Create, FileAccess.Write, FileShare.Read);
        }
    }
}