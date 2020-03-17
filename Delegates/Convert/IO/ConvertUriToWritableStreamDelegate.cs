using System.IO;

using Interfaces.Delegates.Convert;

namespace Delegates.Convert.IO
{
    public class ConvertUriToWritableDelegate : IConvertDelegate<string, Stream>
    {
        public Stream Convert(string uri)
        {
            var directoryName = Path.GetDirectoryName(uri);
            if (!string.IsNullOrEmpty(directoryName) &&
                !System.IO.Directory.Exists(directoryName))
            {
                System.IO.Directory.CreateDirectory(directoryName);
            }

            return new FileStream(uri, FileMode.Create, FileAccess.Write, FileShare.Read);
        }
    }
}