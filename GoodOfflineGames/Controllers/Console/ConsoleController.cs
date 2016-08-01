using Interfaces.Console;

namespace Controllers.Console
{
    public class ConsoleController: IConsoleController
    {
        public string Read()
        {
            return System.Console.Read().ToString();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public string ReadPrivateLine()
        {
            System.ConsoleKeyInfo key;
            string privateData = string.Empty;
            while ((key = System.Console.ReadKey(true)).Key != System.ConsoleKey.Enter)
            {
                privateData += key.KeyChar;
            }
            return privateData;
        }

        public void Write(string message, MessageType messageType, params object[] data)
        {
            System.Console.ForegroundColor = GetColorByMessageType(messageType);
            System.Console.Write(message, data);
        }

        public void WriteLine(string message, MessageType messageType, params object[] data)
        {
            System.Console.ForegroundColor = GetColorByMessageType(messageType);
            System.Console.WriteLine(message, data);
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
                    return System.ConsoleColor.White;
            }
        }

    }
}
