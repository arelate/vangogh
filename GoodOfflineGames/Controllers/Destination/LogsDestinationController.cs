﻿using System;
using Interfaces.Destination;

namespace Controllers.Destination
{
    public class LogsDestinationController : IDestinationController
    {
        private const string logFilenameTemplate = "log-latest.js";

        public string GetDirectory(string source)
        {
            return "logs";
        }

        public string GetFilename(string source)
        {
            return string.Format(logFilenameTemplate, source);
        }
    }
}
