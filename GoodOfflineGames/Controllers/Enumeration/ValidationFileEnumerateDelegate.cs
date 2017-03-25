using System.Collections.Generic;
using System.IO;

using Interfaces.Enumeration;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;

namespace Controllers.Enumeration
{
    public class ValidationFileEnumerateDelegate : IEnumerateDelegate<string>
    {
        private IGetDirectoryDelegate validationDirectoryDelegate;
        private IGetFilenameDelegate validationFilenameDelegate;

        public ValidationFileEnumerateDelegate(
            IGetDirectoryDelegate validationDirectoryDelegate,
            IGetFilenameDelegate validationFilenameDelegate)
        {
            this.validationDirectoryDelegate = validationDirectoryDelegate;
            this.validationFilenameDelegate = validationFilenameDelegate;
        }

        public IEnumerable<string> Enumerate(string uri)
        {
            return new string[] {
                Path.Combine(
                    validationDirectoryDelegate.GetDirectory(),
                    validationFilenameDelegate.GetFilename(
                        Path.GetFileName(uri)))
            };
        }
    }
}
