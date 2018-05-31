using System.IO;
using System.Threading.Tasks;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.Output;
using Interfaces.Controllers.Stream;

using Interfaces.Status;

namespace Controllers.Presentation
{
    // TODO: The amount of NotImplementedException is clear indication this needs to be a delegate instead
    public class FilePresentationController : IOutputController<string[]>
    {
        IGetDirectoryDelegate getDirectoryDelegate;
        IGetFilenameDelegate getFilenameDelegate;
        IStreamController streamController;

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

        public async Task OutputContinuousAsync(IStatus status, params string[] lines)
        {
            var reportUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(string.Empty),
                getFilenameDelegate.GetFilename(string.Empty));

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
