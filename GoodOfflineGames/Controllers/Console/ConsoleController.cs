using System;
using Interfaces.Console;

namespace Controllers.Console
{
    public class ConsoleController : IConsoleController
    {
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
            System.ConsoleKeyInfo key;
            string password = string.Empty;
            while ((key = System.Console.ReadKey(true)).Key != System.ConsoleKey.Enter)
            {
                string output = string.Empty;
                bool passwordIncrement = false;

                switch (key.Key)
                {
                    case System.ConsoleKey.Backspace:
                        if (password.Length > 0)
                            password = password.Substring(0, password.Length - 1);
                        break;
                    case System.ConsoleKey.UpArrow:
                    case System.ConsoleKey.DownArrow:
                    case System.ConsoleKey.LeftArrow:
                    case System.ConsoleKey.RightArrow:
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

        public void Write(string message, MessageType messageType = MessageType.Default, params object[] data)
        {
            System.Console.ForegroundColor = GetColorByMessageType(messageType);
            System.Console.Write(message, data);
        }

        public void WriteLine(string message, MessageType messageType = MessageType.Default, params object[] data)
        {
            System.Console.ForegroundColor = GetColorByMessageType(messageType);
            if (data != null &&
                data.Length > 0)
                System.Console.WriteLine(message, data);
            else
                System.Console.WriteLine(message);
        }

        private System.ConsoleColor GetColorByMessageType(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Error:
                    return System.ConsoleColor.Red;
                case MessageType.Warning:
                    return System.ConsoleColor.Yellow;
                case MessageType.Success:
                    return System.ConsoleColor.Green;
                case MessageType.Progress:
                    return System.ConsoleColor.Gray;
                case MessageType.Default:
                default:
                    return System.ConsoleColor.Gray;
            }
        }

        public void Clear()
        {
            System.Console.Clear();
        }

        public void SetCursorPosition(int left, int top)
        {
            System.Console.SetCursorPosition(left, top);
        }
    }
}
