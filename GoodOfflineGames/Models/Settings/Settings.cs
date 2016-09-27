using System.Runtime.Serialization;

using Interfaces.Settings;

namespace Models.Settings
{
    [DataContract]
    public class ConnectionProperties: IConnectionProperties
    {
        [DataMember(Name = "userAgent")]
        public string UserAgent { get; set; }
    }

    [DataContract]
    public class AuthenticationProperties : IAuthenticationProperties
    {
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }

    [DataContract]
    public class UpdateProperties : IUpdateProperties
    {
        [DataMember(Name = "everything")]
        public bool Everything { get; set; }
    }

    public class DownloadProperties : IDownloadProperties
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

    public class ValidationProperties : IValidationProperties
    {
        [DataMember(Name = "afterDownload")]
        public bool AfterDownload { get; set; }
    }

    public class CleanupProperties : ICleanupProperties
    {
        [DataMember(Name = "afterDownload")]
        public bool AfterDownload { get; set; }
    }

    [DataContract]
    public class Settings
    {
        [DataMember(Name = "connection")]
        public ConnectionProperties Connection { get; set; }

        [DataMember(Name = "authentication")]
        public AuthenticationProperties Authentication { get; set; }

        [DataMember(Name = "update")]
        public UpdateProperties Update { get; set; }

        [DataMember(Name = "download")]
        public DownloadProperties Download { get; set; }

        [DataMember(Name = "validation")]
        public ValidationProperties Validation { get; set; }

        [DataMember(Name = "cleanup")]
        public CleanupProperties Cleanup { get; set; }
    }
}
