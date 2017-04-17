﻿using Interfaces.PropertyValidation;
using Interfaces.Console;

namespace Controllers.PropertyValidation
{
    public class SecurityCodeValidationDelegate : IValidatePropertiesDelegate<string>
    {
        private const string securityCodeHasBeenSent = "Enter four digits security code that has been sent to your email:";
        private IConsoleController consoleController;

        public SecurityCodeValidationDelegate(IConsoleController consoleController)
        {
            this.consoleController = consoleController;
        }

        public string[] ValidateProperties(params string[] properties)
        {
            var securityCode = string.Empty;
            consoleController.Clear();

            while (securityCode.Length != 4)
            {
                consoleController.WriteLine(securityCodeHasBeenSent);
                securityCode = consoleController.ReadLine();
            }

            return new string[] { securityCode };
        }
    }
}