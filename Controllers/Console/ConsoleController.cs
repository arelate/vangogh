using System;
using System.Collections.Generic;

using Interfaces.Console;

using Models.Separators;

namespace Controllers.Console
{
    public class ConsoleController : IConsoleController
    {
        public int WindowWidth
        {
            get { return System.Console.WindowWidth; }
        }

        public int WindowHeight
        {
            get { return System.Console.WindowHeight; }
        }

        public string Read()
        {
            return System.Console.Read().ToString();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public string InputPassword()
        {
            ConsoleKeyInfo key;
            string password = string.Empty;
            while ((key = System.Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                string output = string.Empty;
                bool passwordIncrement = false;

                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        if (password.Length > 0)
                            password = password.Substring(0, password.Length - 1);
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                        continue;
                    default:
                        password += key.KeyChar;
                        passwordIncrement = true;
                        break;
                }

                // clear the line
                System.Console.Write("\r" + new string(' ', password.Length + 1) + "\r");

                var length = passwordIncrement ? password.Length - 1 : password.Length;
                System.Console.Write(new string('*', length));
                if (passwordIncrement && password.Length > 0)
                    System.Console.Write(password[password.Length - 1]);
            }
            return password;
        }

        public void Write(string message, params object[] data)
        {
            OutputFormattedMessage(System.Console.Write, message, data);
        }

        public void WriteLine(string message, params object[] data)
        {
            OutputFormattedMessage(System.Console.WriteLine, message, data);
        }

        private void OutputFormattedMessage(Action<string> consoleOutput, string message, params object[] data)
        {
            var formattedMessage =
                (data != null && data.Length > 0) ?
                string.Format(message, data) :
                message;

            consoleOutput(formattedMessage);
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }
    }
}
