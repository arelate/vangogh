using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Delegates.Hash;

using Interfaces.Status;
using Interfaces.Validation;

namespace Controllers.Validation
{
    public class DataFileValidateDelegate : IValidateFileAsyncDelegate<bool>
    {
        readonly IGetHashAsyncDelegate<string> getFileMd5HashAsyncDelegate;
        IStatusController statusController;

        public DataFileValidateDelegate(
            IGetHashAsyncDelegate<string> getFileMd5HashAsyncDelegate,
            IStatusController statusController)
        {
            this.getFileMd5HashAsyncDelegate = getFileMd5HashAsyncDelegate;
            this.statusController = statusController;
        }

        public async Task<bool> ValidateFileAsync(string uri, string md5, IStatus status)
        {
            return await getFileMd5HashAsyncDelegate.GetHashAsync(uri, status) == md5;
        }
    }
}
