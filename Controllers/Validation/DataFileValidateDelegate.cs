using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Controllers.Hash;

using Interfaces.Status;
using Interfaces.Validation;

namespace Controllers.Validation
{
    public class DataFileValidateDelegate : IValidateFileAsyncDelegate<bool>
    {
        private IFileHashController fileMd5Controller;
        private IStatusController statusController;

        public DataFileValidateDelegate(
            IFileHashController fileMd5Controller,
            IStatusController statusController)
        {
            this.fileMd5Controller = fileMd5Controller;
            this.statusController = statusController;
        }

        public async Task<bool> ValidateFileAsync(string uri, string md5, IStatus status)
        {
            return await fileMd5Controller.GetHashAsync(uri) == md5;
        }
    }
}
