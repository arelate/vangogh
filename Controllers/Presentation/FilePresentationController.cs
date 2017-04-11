using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

using Interfaces.Presentation;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Stream;

namespace Controllers.Presentation
{
    public class FilePresentationController : IPresentationController<string>
    {
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IStreamController streamController;

        public FilePresentationController(
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            IStreamController streamController)
        {
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.streamController = streamController;
        }

        public void Present(IEnumerable<string> views)
        {
            throw new NotImplementedException();
        }

        public async Task PresentAsync(IEnumerable<string> views)
        {
            var reportUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            using (var reportStream = streamController.OpenWritable(reportUri))
                using (var streamWriter = new StreamWriter(reportStream))
                    foreach (var view in views)
                        await streamWriter.WriteLineAsync(view);
        }
    }
}
