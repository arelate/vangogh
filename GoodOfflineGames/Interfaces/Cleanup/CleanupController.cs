using Interfaces.Reporting;

namespace Interfaces.Cleanup
{
    public interface ICleanupDelegate
    {
        int Cleanup(string removeToFolder, IReportUpdateDelegate reportUpdateDelegate = null);
    }

    public interface ICleanupValidationFileDelegate
    {
        bool CleanupValidationFile(string localFile, string removeToFolder);
    }

    public interface ICleanupController :
        ICleanupDelegate,
        ICleanupValidationFileDelegate
    {
        // ...
    }
}
