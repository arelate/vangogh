using System;

using GOG.Interfaces;

namespace GOG.SharedControllers
{
    class ConsoleController : IDisposableConsoleController
    {
        public void Dispose()
        {
            // normal consoleController has nothing to dispose of
        }

        public string Read()
        {
            return Console.Read().ToString();
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public string ReadPrivateLine()
        {
            ConsoleKeyInfo key;
            string privateData = string.Empty;
            while ((key = Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                privateData += key.KeyChar;
            }
            return privateData;
        }

        public void Write(string message, ConsoleColor color, params object[] data)
        {
            Console.ForegroundColor = color;
            Console.Write(message, data);
        }

        public void WriteLine(string message, ConsoleColor color, params object[] data)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message, data);
        }
    }
}
