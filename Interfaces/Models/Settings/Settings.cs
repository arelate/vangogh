using System.Collections.Generic;

namespace Interfaces.Models.Settings
{
    public interface ISettings
    {
        string Username { get; set; }
        string Password { get; set; }
        string[] DownloadsLanguages { get; set; }
        string[] DownloadsOperatingSystems { get; set; }
        IDictionary<string, string> Directories { get; set; }
    }
}