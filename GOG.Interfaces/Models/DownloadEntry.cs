namespace GOG.Interfaces.Models
{
    public interface IDownloadEntry
    {
        string Date { get; set; }
        string ManualUrl { get; set; }
        string DownloaderUrl { get; set; }
        string Name { get; set; }
        string Type { get; set; }
        int Info { get; set; }
        string Size { get; set; }
        string Version { get; set; }
    }
}
