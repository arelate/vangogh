using System;
using Interfaces.Delegates.Data;

namespace Delegates.Data.Console
{
    public class GetPrivateLineDataDelegate : IGetDataDelegate<string>
    {
        public string GetData(string message = null)
        {
            System.Console.WriteLine(message);

            ConsoleKeyInfo key;
            var privateLine = string.Empty;
            while ((key = System.Console.ReadKey(true)).Key != ConsoleKey.Enter)
            {
                var output = string.Empty;
                var privateLineIncrement = false;

                switch (key.Key)
                {
                    case ConsoleKey.Backspace:
                        if (privateLine.Length > 0)
                            privateLine = privateLine.Substring(0, privateLine.Length - 1);
                        break;
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.DownArrow:
                    case ConsoleKey.LeftArrow:
                    case ConsoleKey.RightArrow:
                        continue;
                    default:
                        privateLine += key.KeyChar;
                        privateLineIncrement = true;
                        break;
                }

                // clear the line
                System.Console.Write("\r" + new string(' ', privateLine.Length + 1) + "\r");

                var length = privateLineIncrement ? privateLine.Length - 1 : privateLine.Length;
                System.Console.Write(new string('*', length));
                if (privateLineIncrement && privateLine.Length > 0)
                    System.Console.Write(privateLine[^1]);
            }

            System.Console.WriteLine(string.Empty);
            return privateLine;
        }
    }
}