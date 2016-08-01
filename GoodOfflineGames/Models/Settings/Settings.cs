using System.Runtime.Serialization;

using Interfaces.Settings;

namespace Models.Settings
{
    [DataContract]
    public class SettingsAuthenticate : IAuthenticateProperties
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

    [DataContract]
    public class SettingsUpdate : IUpdateProperties
    {
        [DataMember(Name = "everything")]
        public bool Everything { get; set; }
    }

    public class SettingsDownload : IDownloadProperties
    {
        [DataMember(Name = "images")]
        public bool Images { get; set; }
        [DataMember(Name = "screenshots")]
        public bool Screenshots { get; set; }
        [DataMember(Name = "files")]
        public bool Files { get; set; }
        [DataMember(Name = "langugages")]
        public string[] Languages { get; set; }
        [DataMember(Name = "operatingSystems")]
        public string[] OperatingSystems { get; set; }
    }

    public class SettingsValidate : IValidateProperties
    {
        [DataMember(Name = "afterDownload")]
        public bool AfterDownload { get; set; }
    }

    public class SettingsCleanup : ICleanupProperties
    {
        [DataMember(Name = "afterDownload")]
        public bool AfterDownload { get; set; }
    }

    [DataContract]
    public class Settings
    {
        [DataMember(Name = "authenticate")]
        public SettingsAuthenticate Authenticate { get; set; }

        [DataMember(Name = "update")]
        public SettingsUpdate Update { get; set; }

        [DataMember(Name = "download")]
        public SettingsDownload Download { get; set; }

        [DataMember(Name = "validate")]
        public SettingsValidate Validate { get; set; }

        [DataMember(Name = "cleanup")]
        public SettingsCleanup Cleanup { get; set; }

        public Settings()
        {
            Authenticate = new SettingsAuthenticate();
            Update = new SettingsUpdate();
            Validate = new SettingsValidate();
            Cleanup = new SettingsCleanup();
        }
    }
}
