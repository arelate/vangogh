using System.IO;
using System.Threading.Tasks;

using Interfaces.Output;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.Stream;

namespace Controllers.Presentation
{
    public class FilePresentationController : IOutputController<string[]>
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

        public Task ClearContinuousLinesAsync(int lines)
        {
            throw new System.NotImplementedException();
        }

        public async Task OutputContinuousAsync(params string[] lines)
        {
            var reportUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            using (var reportStream = streamController.OpenWritable(reportUri))
            using (var streamWriter = new StreamWriter(reportStream))
                foreach (var line in lines)
                    await streamWriter.WriteLineAsync(line);
        }

        public Task OutputFixedOnRefreshAsync(string[] data)
        {
            throw new System.NotImplementedException();
        }

        public Task OutputOnRefreshAsync(string[] data)
        {
            throw new System.NotImplementedException();
        }

        public Task SetRefreshAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
