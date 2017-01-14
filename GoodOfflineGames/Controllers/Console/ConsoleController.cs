using System;
using System.Collections.Generic;

using Interfaces.Console;

using Models.Separators;

namespace Controllers.Console
{
    public class ConsoleController : IConsoleController
    {
        public ConsoleController()
        {
            DefaultColor = ConsoleColor.Gray;
        }

        public ConsoleColor DefaultColor { get; set; }

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

        public void Write(string message, string[] colors = null, params object[] data)
        {
            OutputMessage(System.Console.Write, message, colors, data);
        }

        public void WriteLine(string message, string[] colors = null, params object[] data)
        {
            OutputMessage(System.Console.WriteLine, message, colors, data);
        }

        private ConsoleColor ParseColor(string color)
        {
            var consoleColor = DefaultColor;
            Enum.TryParse(color, true, out consoleColor);

            if (consoleColor == ConsoleColor.Black &&
                color.ToLower() != "black")
                consoleColor = DefaultColor; 

            return consoleColor;
        }

        private void OutputMessage(Action<string> consoleOutput, string message, string[] colors = null, params object[] data)
        {
            System.Console.ForegroundColor = DefaultColor;

            var formattedMessage =
                (data != null && data.Length > 0) ?
                string.Format(message, data) :
                message;

            if (colors == null)
            {
                consoleOutput(formattedMessage);
                return;
            }

            var computedColors = new List<ConsoleColor>() { DefaultColor };
            foreach (var color in colors)
                computedColors.Add(ParseColor(color));

            var formattedMessageParts = formattedMessage.Split(
                new string[1] { Separators.ConsoleColor }, 
                StringSplitOptions.None);

            if (formattedMessageParts.Length > computedColors.Count)
                throw new FormatException("Number of colors is less than required to format the message");

            for (var ii=0; ii<formattedMessageParts.Length; ii++)
            {
                System.Console.ForegroundColor = computedColors[ii];
                System.Console.Write(formattedMessageParts[ii]);
            }

            consoleOutput(string.Empty);
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
