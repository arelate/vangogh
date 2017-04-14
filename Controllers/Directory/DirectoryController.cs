using System;
using System.IO;
using System.Collections.Generic;

using Interfaces.Directory;

namespace Controllers.Directory
{
    public class DirectoryController : IDirectoryController
    {
        public bool Exists(string uri)
        {
            return System.IO.Directory.Exists(uri);
        }

        public void Create(string uri)
        {
            System.IO.Directory.CreateDirectory(uri);
        }

        public void Delete(string uri)
        {
            System.IO.Directory.Delete(uri, true);
        }

        public IEnumerable<string> EnumerateFiles(string uri)
        {
            return System.IO.Directory.EnumerateFiles(uri);
        }

        public IEnumerable<string> EnumerateDirectories(string uri)
        {
            return System.IO.Directory.EnumerateDirectories(uri);
        }

        public void Move(string fromUri, string toUri)
        {
            var destination = Path.Combine(toUri, fromUri);
            if (!Exists(Path.GetDirectoryName(destination)))
                Create(Path.GetDirectoryName(destination));
            System.IO.Directory.Move(
                fromUri,
                destination);
        }

        public void SetCurrentDirectory(string uri)
        {
            if (Exists(uri))
                System.IO.Directory.SetCurrentDirectory(uri);
        }
    }
}
