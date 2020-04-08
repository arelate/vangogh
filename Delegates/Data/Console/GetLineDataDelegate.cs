using Interfaces.Delegates.Data;

namespace Delegates.Data.Console
{
    public class GetLineDataDelegate : IGetDataDelegate<string>
    {
        public string GetData(string message = null)
        {
            System.Console.WriteLine(message);
            return System.Console.ReadLine();
        }
    }
}