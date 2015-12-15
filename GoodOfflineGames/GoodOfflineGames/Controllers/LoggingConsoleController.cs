using System;
using GOG.Interfaces;
using System.IO;

namespace GOG.SharedControllers
{
    class LoggingConsoleController : IConsoleController
    {
        private string logFilename;
        private IConsoleController consoleController;

        private StreamWriter logStreamWriter;

        public LoggingConsoleController(string logFilename, IConsoleController consoleController)
        {
            this.logFilename = logFilename;
            this.consoleController = consoleController;

            logStreamWriter = new StreamWriter(logFilename, true, System.Text.Encoding.UTF8);
            logStreamWriter.AutoFlush = true;
        }

        public string Read()
        {
            return consoleController.Read();
        }

        public string ReadLine()
        {
            return consoleController.ReadLine();
        }

        public string ReadPrivateLine()
        {
            return consoleController.ReadPrivateLine();
        }

        public void Write(string message, params object[] data)
        {
            logStreamWriter.Write(message, data);
            consoleController.Write(message, data);
        }

        public void WriteLine(string message, params object[] data)
        {
            logStreamWriter.WriteLine(message, data);
            consoleController.WriteLine(message, data);
        }

        public void Dispose()
        {
            if (logStreamWriter != null) logStreamWriter.Dispose();
        }
    }
}
