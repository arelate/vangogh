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
    public class FilePresentationController : IPresentationController<string[]>
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

        public void Present(params string[] lines)
        {
            throw new NotImplementedException();
        }

        public async Task PresentAsync(params string[] lines)
        {
            var reportUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            using (var reportStream = streamController.OpenWritable(reportUri))
            using (var streamWriter = new StreamWriter(reportStream))
                foreach (var line in lines)
                    await streamWriter.WriteLineAsync(line);
        }
    }
}
