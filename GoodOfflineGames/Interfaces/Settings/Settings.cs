namespace Interfaces.Settings
{
    public interface ISettings
    {
        string Username { get; set; }
        string Password { get; set; }
        string[] DownloadsLanguages { get; set; }
        string[] DownloadsOperatingSystems { get; set; }
    }

    public interface ISettingsProperty
    {
        ISettings Settings { get; }
    }
}
