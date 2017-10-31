using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.RequestData;
using Interfaces.Console;

namespace Controllers.RequestData
{
    public class RequestDataController : IRequestDataController<string>
    {
        private IConsoleController consoleController;

        public RequestDataController(IConsoleController consoleController)
        {
            this.consoleController = consoleController; 
        }

        public string RequestData(string message)
        {
            consoleController.WriteLine(message);
            return consoleController.ReadLine();
        }

        public string RequestPrivateData(string message)
        {
            consoleController.WriteLine(message);
            return consoleController.InputPassword();
        }
    }
}
