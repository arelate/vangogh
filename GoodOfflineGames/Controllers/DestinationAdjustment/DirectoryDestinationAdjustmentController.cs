using System;
using System.Collections.Generic;

using Interfaces.DestinationAdjustment;

namespace Controllers.DestinationAdjustment
{
    public class DirectoryDestinationAdjustmentController : IDestinationAdjustmentController
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

        public string AdjustDestination(string source)
        {
            var dirIndex = uriStructure.IndexOf(directory);
            var uri = new Uri(source);
            var directoryWithTrailingSlash = uri.Segments[dirIndex];
            var uriDirectory = directoryWithTrailingSlash.Substring(0, directoryWithTrailingSlash.Length - 1);
            return uriDirectory;
        }
    }
}
