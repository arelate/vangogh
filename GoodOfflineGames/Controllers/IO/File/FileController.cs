using System;
using System.IO;

using Interfaces.IO.File;

namespace Controllers.IO.File
{
    public class FileController: IFileController
    {
        public bool Exists(string uri)
        {
            return System.IO.File.Exists(uri);
        }

        public void Move(string fromUri, string toUri)
        {
            System.IO.File.Move(fromUri, toUri);
        }

        public long GetSize(string uri)
        {
            var fileInfo = new FileInfo(uri);
            return fileInfo.Length;
        }

        public DateTime GetTimestamp(string uri)
        {
            var fileInfo = new FileInfo(uri);
            return fileInfo.CreationTimeUtc;
        }

    }
}
