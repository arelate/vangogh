using System;
using System.Collections.Generic;
using System.Text;

namespace Ghost.Console
{
    class PlaygroundProgram
    {
        static async void Main(string[] args)
        {
            var consoleController = new Controllers.Console.ConsoleController();
            var lineBreakingDelegate = new Controllers.LineBreaking.LineBreakingDelegate();

            var consoleIOController = new Controllers.InputOutput.ConsoleInputOutputController(
                lineBreakingDelegate,
                consoleController);

            await consoleIOController.OutputOnRefreshAsync("abc", "de", "fgh");
            await consoleIOController.OutputContinuousAsync("v", "uw", "xyz");

            var name = await consoleIOController.RequestInputAsync("Enter some name:");

            await consoleIOController.OutputFixedOnRefreshAsync("this is fixed content");

            await consoleIOController.OutputOnRefreshAsync("abc", "de", "fgh");

            await consoleIOController.RequestPrivateInputAsync(string.Format("Enter password for {0}", name));

            await consoleIOController.OutputFixedOnRefreshAsync("and some more fixed content");

            await consoleIOController.OutputOnRefreshAsync("v", "uw");

            System.Console.ReadLine();

        }
    }
}
