namespace Interfaces.Settings
{
    public interface ISettings
    {
        bool ContinueExistingSession { get; set; }
        string Username { get; set; }
        string Password { get; set; }
        string[] UpdateData { get; set; }
        string[] DownloadsLanguages { get; set; }
        string[] DownloadsOperatingSystems { get; set; }
        string[] UpdateDownloads { get; set; }
        string[] ProcessDownloads { get; set; }
        bool Validate { get; set; }
        string[] Cleanup { get; set; }
        bool DiagnosticsLog { get; set; }
    }

    public interface ISettingsProperty
    {
        ISettings Settings { get; }
    }
}
