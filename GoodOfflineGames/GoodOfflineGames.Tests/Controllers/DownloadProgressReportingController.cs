using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;

using GOG.Interfaces;
using GOG.Controllers;

namespace GoodOfflineGames.Tests.Controllers
{
    class ReportingConsole : IConsoleController
    {
        public string Read()
        {
            throw new NotImplementedException();
        }

        public string ReadLine()
        {
            throw new NotImplementedException();
        }

        public string ReadPrivateLine()
        {
            throw new NotImplementedException();
        }

        public void Write(string message, ConsoleColor color, params object[] data)
        {
            LastSetValue = string.Format(message, data);
        }

        public void WriteLine(string message, ConsoleColor color, params object[] data)
        {
            LastSetValue = string.Format(message, data);
        }

        public string LastSetValue { get; set; }
    }

    [TestClass]
    public class DownloadProgressReportingControllerTests
    {
        private IDownloadProgressReportingController downloadProgressReportingController;
        private IFormattingController bytesFormattingController;
        private IFormattingController secondsFormattingController;
        private ReportingConsole consoleController;

        public DownloadProgressReportingControllerTests()
        {
            bytesFormattingController = new BytesFormattingController();
            secondsFormattingController = new SecondsFormattingController();

            consoleController = new ReportingConsole();

            downloadProgressReportingController = new DownloadProgressReportingController(bytesFormattingController, secondsFormattingController, consoleController);

            downloadProgressReportingController.ThrottleMilliseconds = 1;
        }

        [TestMethod]
        public void DownloadProgress()
        {
            const int cycles = 10;

            var expectedOutput = new string[cycles+1]
            {
                "\r0.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r10.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r20.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r30.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r40.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r50.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r60.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r70.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r80.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r90.0 % at zero bytes/s. Estimated time remaining: zero seconds",
                "\r100.0 % at zero bytes/s. Estimated time remaining: zero seconds"
            };

            downloadProgressReportingController.Initialize();

            for (var ii = 0; ii < cycles + 1; ii++)
            {
                Thread.Sleep(1);
                downloadProgressReportingController.Report(ii, cycles);

                Assert.AreEqual(consoleController.LastSetValue, expectedOutput[ii]);
            }

        }
    }
}
