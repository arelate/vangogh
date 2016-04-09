using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG
{
    public class ConsolePostUpdate : IPostUpdateDelegate
    {
        private IConsoleController consoleController;

        public ConsolePostUpdate(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public void PostUpdate()
        {
            consoleController.Write(".", ConsoleColor.Gray);
        }
    }
}
