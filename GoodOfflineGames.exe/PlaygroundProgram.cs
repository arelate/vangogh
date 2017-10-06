using System;
using System.Collections.Generic;
using System.Text;

namespace GoodOfflineGames
{
    class PlaygroundProgram
    {
        static void Main(string[] args)
        {
            var lineBreakingDelegate = new Controllers.LineBreaking.LineBreakingDelegate();
            var consoleController = new Controllers.Console.ConsoleController();
            var consolePresentationController = new Controllers.Presentation.ConsolePresentationController(lineBreakingDelegate, consoleController);

            consolePresentationController.Present(new string[] { "123", "45678", "90" });
            //Console.ReadLine();

            consolePresentationController.Present(new string[] { "123456", "7890" });

            consolePresentationController.Present(new string[] { "123", "4567890" });




            Console.ReadLine();

        }
    }
}
