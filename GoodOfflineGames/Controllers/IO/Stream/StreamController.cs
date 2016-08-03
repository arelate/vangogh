using System.IO;

using Interfaces.IO.Stream;

namespace Controllers.IO.Stream
{
    public class StreamController: IStreamController
    {
        public System.IO.Stream OpenReadable(string uri)
        {
            return new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public System.IO.Stream OpenWritable(string uri)
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
