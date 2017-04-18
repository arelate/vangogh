using System;
using System.Net;
using System.Threading.Tasks;

using Interfaces.Cookies;
using Interfaces.Destination.Filename;
using Interfaces.Storage;

namespace Controllers.Cookies
{
    public class CookieContainerSerializationController : ICookieContainerSerializationController
    {
        private IStorageController<string> storageController;
        private IGetFilenameDelegate filenameDelegate;
        private CookieContainer cookieContainer;

        public CookieContainerSerializationController(
            ref CookieContainer cookieContainer,
            IGetFilenameDelegate filenameDelegate,
            IStorageController<string> storageController)
        {
            this.cookieContainer = cookieContainer;
            this.filenameDelegate = filenameDelegate;
            this.storageController = storageController;
        }

        public async Task LoadAsync()
        {
            throw new NotImplementedException();
        }

        public async Task SaveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
