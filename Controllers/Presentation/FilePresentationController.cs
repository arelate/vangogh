using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.Output;
using Interfaces.Controllers.Stream;

using Interfaces.Status;

namespace Controllers.Presentation
{
    public class FilePresentationController : IOutputController<string[]>
    {
        private IGetDirectoryAsyncDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private IStreamController streamController;

        public FilePresentationController(
            IGetDirectoryAsyncDelegate getDirectoryDelegate,
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

        public async Task OutputContinuousAsync(IStatus status, params string[] lines)
        {
            var reportUri = Path.Combine(
                await getDirectoryDelegate.GetDirectoryAsync(string.Empty, status),
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
