using System;
using System.IO;
using System.Collections.Generic;

using GOG.Interfaces;

namespace GOG.SharedControllers
{
    class IOController : IIOController
    {
        public Stream OpenReadable(string uri)
        {
            return new FileStream(uri, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public Stream OpenWritable(string uri)
        {
            var directoryName = Path.GetDirectoryName(uri);
            if (!string.IsNullOrEmpty(directoryName) &&
                !Directory.Exists(directoryName)) 
            {
                Directory.CreateDirectory(directoryName);
            }

            return new FileStream(uri, FileMode.Create, FileAccess.Write, FileShare.Read);
        }

        public bool FileExists(string uri)
        {
            return File.Exists(uri);
        }

        public void MoveFile(string fromUri, string toUri)
        {
            File.Move(fromUri, toUri);
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

        public bool DirectoryExists(string uri)
        {
            return Directory.Exists(uri);
        }

        public void CreateDirectory(string uri)
        {
            Directory.CreateDirectory(uri);
        }

        public IEnumerable<string> EnumerateFiles(string uri)
        {
            return Directory.EnumerateFiles(uri);
        }
    }
}
