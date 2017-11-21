using System;
using System.Collections.Generic;
using System.Text;

namespace GoodOfflineGames
{
    class PlaygroundProgram
    {
        static void Main(string[] args)
        {
            var consoleController = new Controllers.Console.ConsoleController();
            var lineBreakingDelegate = new Controllers.LineBreaking.LineBreakingDelegate();

            var consoleRequestPresentController = new Controllers.RequestPresent.ConsoleRequestPresentController(
                lineBreakingDelegate,
                consoleController);

            consoleRequestPresentController.PresentNew("abc", "de", "fgh");
            consoleRequestPresentController.PresentAdditional("v", "uw", "xyz");

            consoleRequestPresentController.PresentNew("abc", "de", "fgh");
            consoleRequestPresentController.PresentNew("v", "uw", "xyz");


            Console.ReadLine();

        }
    }
}
