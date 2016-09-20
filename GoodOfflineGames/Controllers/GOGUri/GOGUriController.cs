using System;
using System.Collections.Generic;

using Interfaces.GOGUri;

namespace Controllers.GOGUri
{
    public class GOGUriController : IGOGUriController
    {
        private const char root = 'r',
            secure = 's',
            directory = 'd',
            operatingSystem = 'o',
            file = 'f';
        private readonly List<char> uriStructure = new List<char>(5) {
            root, // /
            secure, // secure/
            directory, // {directory}
            operatingSystem, // {win, mac, linux}
            file }; // {filename.exe}

        public string GetDirectory(string source)
        {
            return GetUriPart(source, directory);
        }

        public string GetFilename(string source)
        {
            return GetUriPart(source, file);
        }

        private string GetUriPart(string source, char part)
        {
            var partIndex = uriStructure.IndexOf(part);
            var uri = new Uri(source);
            var uriPart = uri.Segments[partIndex];
            return uriPart;
        }
    }
}
