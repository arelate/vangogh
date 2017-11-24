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

            var consoleIOController = new Controllers.InputOutput.ConsoleInputOutputController(
                lineBreakingDelegate,
                consoleController);

            consoleIOController.OutputOnRefresh("abc", "de", "fgh");
            consoleIOController.OutputContinuous("v", "uw", "xyz");

            var name = consoleIOController.RequestInput("Enter some name:");

            consoleIOController.OutputFixedOnRefresh("this is fixed content");

            consoleIOController.OutputOnRefresh("abc", "de", "fgh");

            consoleIOController.RequestPrivateInput(string.Format("Enter password for {0}", name));

            consoleIOController.OutputFixedOnRefresh("and some more fixed content");

            consoleIOController.OutputOnRefresh("v", "uw");

            Console.ReadLine();

        }
    }
}
