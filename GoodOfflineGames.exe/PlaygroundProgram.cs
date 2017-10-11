using System;
using System.Collections.Generic;
using System.Text;

namespace GoodOfflineGames
{
    class PlaygroundProgram
    {
        static void Main(string[] args)
        {
            var userRequestedController = new Controllers.UserRequested.UserRequestedController("123", "1", "a");

            Console.WriteLine(userRequestedController.IsNullOrEmpty());
            foreach (var id in userRequestedController.EnumerateIds())
                Console.WriteLine(id);

            Console.ReadLine();

        }
    }
}
