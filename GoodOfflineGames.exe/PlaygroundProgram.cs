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

            consoleIOController.OutputOnRefresh("abc", "de", "fgh");
            consoleIOController.OutputOnRefresh("v", "uw", "xyz");

            Console.ReadLine();

        }
    }
}
