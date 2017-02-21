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
        [DataMember(Name = "products")]
        public bool Products { get; set; }
        [DataMember(Name = "accountProducts")]
        public bool AccountProducts { get; set; }
        [DataMember(Name = "wishlist")]
        public bool Wishlist { get; set; }
        [DataMember(Name = "gameProductData")]
        public bool GameProductData { get; set; }
        [DataMember(Name = "apiProducts")]
        public bool ApiProducts { get; set; }
        [DataMember(Name = "gameDetails")]
        public bool GameDetails { get; set; }
        [DataMember(Name = "screenshots")]
        public bool Screenshots { get; set; }
    }

    [DataContract]
    public class DownloadProperties : IDownloadProperties
    {
        [DataMember(Name = "productsImages")]
        public bool ProductsImages { get; set; }
        [DataMember(Name = "accountProductsImages")]
        public bool AccountProductsImages { get; set; }        
        [DataMember(Name = "screenshots")]
        public bool Screenshots { get; set; }
        [DataMember(Name = "productsFiles")]
        public bool ProductsFiles { get; set; }
        [DataMember(Name = "languages")]
        public string[] Languages { get; set; }
        [DataMember(Name = "operatingSystems")]
        public string[] OperatingSystems { get; set; }
    }

    [DataContract]
    public class ValidationProperties: IValidationProperties
    {
        [DataMember(Name = "download")]
        public bool Download { get; set; }
        [DataMember(Name = "validateUpdated")]
        public bool ValidateUpdated { get; set; }
    }

    [DataContract]
    public class CleanupProperties : ICleanupProperties
    {
        [DataMember(Name = "directories")]
        public bool Directories { get; set; }
        [DataMember(Name = "files")]
        public bool Files { get; set; }        
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

        [DataMember(Name = "log")]
        public bool Log { get; set; }
    }
}
