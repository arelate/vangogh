using System;
using System.IO;

using Interfaces.File;

namespace Controllers.File
{
    public class FileController: IFileController
    {
        private string recycleBinUri;

        public FileController(string recycleBinUri)
        {
            this.recycleBinUri = recycleBinUri;
        }

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

        public void MoveToRecycleBin(string uri)
        {
            Move(uri, recycleBinUri);
        }
    }
}
