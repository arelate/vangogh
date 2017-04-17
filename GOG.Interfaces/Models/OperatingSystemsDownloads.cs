namespace GOG.Interfaces.Models
{
    public interface IOperatingSystemsDownloads
    {
        string Language { get; set; }
        IDownloadEntry[] Linux { get; set; }
        IDownloadEntry[] Mac { get; set; }
        IDownloadEntry[] Windows { get; set; }
    }
}
