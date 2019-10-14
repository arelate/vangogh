using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Status;
using Interfaces.Validation;

namespace Controllers.Validation
{
    public class DataFileValidateDelegate : IValidateFileAsyncDelegate<bool>
    {
        readonly IConvertAsyncDelegate<string, Task<string>> convertFileToMd5HashDelegate;
        IStatusController statusController;

        public DataFileValidateDelegate(
            IConvertAsyncDelegate<string, Task<string>> convertFileToMd5HashDelegate,
            IStatusController statusController)
        {
            this.convertFileToMd5HashDelegate = convertFileToMd5HashDelegate;
            this.statusController = statusController;
        }

        public async Task<bool> ValidateFileAsync(string uri, string md5, IStatus status)
        {
            return await convertFileToMd5HashDelegate.ConvertAsync(uri, status) == md5;
        }
    }
}
