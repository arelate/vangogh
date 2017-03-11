using System;
using System.Runtime.Serialization;

using Interfaces.Settings;

namespace Models.Settings
{
    //[DataContract]
    //public class ConnectionProperties: IConnectionProperties
    //{
    //    [DataMember(Name = "userAgent")]
    //    public string UserAgent { get; private set; }
    //}

    //[DataContract]
    //public class AuthorizeProperties : IAuthorizeProperties
    //{
    //    [DataMember(Name = "username")]
    //    public string Username { get; private set; }
    //    [DataMember(Name = "password")]
    //    public string Password { get; private set; }
    //}

    //[DataContract]
    //public class UpdateDataProperties : IUpdateDataProperties
    //{
    //    [DataMember(Name = "products")]
    //    public bool Products { get; private set; }
    //    [DataMember(Name = "accountProducts")]
    //    public bool AccountProducts { get; private set; }
    //    [DataMember(Name = "wishlist")]
    //    public bool Wishlist { get; private set; }
    //    [DataMember(Name = "gameProductData")]
    //    public bool GameProductData { get; private set; }
    //    [DataMember(Name = "apiProducts")]
    //    public bool ApiProducts { get; private set; }
    //    [DataMember(Name = "gameDetails")]
    //    public bool GameDetails { get; private set; }
    //    [DataMember(Name = "screenshots")]
    //    public bool Screenshots { get; private set; }
    //}

    //[DataContract]
    //public class UpdateDownloadsProperties : IUpdateDownloadsProperties
    //{
    //    [DataMember(Name = "productsImages")]
    //    public bool ProductsImages { get; private set; }
    //    [DataMember(Name = "accountProductsImages")]
    //    public bool AccountProductsImages { get; private set; }
    //    [DataMember(Name = "screenshots")]
    //    public bool Screenshots { get; private set; }
    //    [DataMember(Name = "productsFiles")]
    //    public bool ProductsFiles { get; private set; }
    //    [DataMember(Name = "validationFiles")]
    //    public bool ValidationFiles { get; private set; }
    //    [DataMember(Name = "languages")]
    //    public string[] Languages { get; private set; }
    //    [DataMember(Name = "operatingSystems")]
    //    public string[] OperatingSystems { get; private set; }
    //}

    //[DataContract]
    //public class ProcessDownloadsProperties : IProcessDownloadsProperties
    //{
    //    [DataMember(Name = "accountProductsImages")]
    //    public bool AccountProductsImages { get; private set; }
    //    [DataMember(Name = "productsFiles")]
    //    public bool ProductsFiles { get; private set; }
    //    [DataMember(Name = "productsImages")]
    //    public bool ProductsImages { get; private set; }
    //    [DataMember(Name = "screenshots")]
    //    public bool Screenshots { get; private set; }
    //    [DataMember(Name = "validationFiles")]
    //    public bool ValidationFiles { get; private set; }
    //}

    //[DataContract]
    //public class ValidateProperties : IValidateProperties
    //{
    //    [DataMember(Name = "validateUpdated")]
    //    public bool ValidateUpdated { get; private set; }
    //}

    //[DataContract]
    //public class CleanupProperties : ICleanupProperties
    //{
    //    [DataMember(Name = "directories")]
    //    public bool Directories { get; private set; }
    //    [DataMember(Name = "files")]
    //    public bool Files { get; private set; }
    //}

    //[DataContract]
    //public class Settings
    //{
    //    [DataMember(Name = "connection")]
    //    public ConnectionProperties Connection { get; private set; }

    //    [DataMember(Name = "authentication")]
    //    public AuthorizeProperties Authorize { get; private set; }

    //    [DataMember(Name = "update")]
    //    public UpdateDataProperties UpdateData { get; private set; }

    //    [DataMember(Name = "updateDownloads")]
    //    public UpdateDownloadsProperties UpdateDownloads { get; private set; }

    //    [DataMember(Name = "processDownloads")]
    //    public ProcessDownloadsProperties ProcessDownloads { get; private set; }

    //    [DataMember(Name = "validation")]
    //    public ValidateProperties Validate { get; private set; }

    //    [DataMember(Name = "cleanup")]
    //    public CleanupProperties Cleanup { get; private set; }

    //    [DataMember(Name = "log")]
    //    public bool Log { get; private set; }
    //}

    [DataContract]
    public class Settings: ISettings
    {
        [DataMember(Name = "continueExistingSession")]
        public bool ContinueExistingSession { get; set; }
        [DataMember(Name = "username")]
        public string Username { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "updateData")]
        public string[] UpdateData { get; set; }
        [DataMember(Name = "downloadsLanguages")]
        public string[] DownloadsLanguages { get; set; }
        [DataMember(Name = "downloadsOperatingSystems")]
        public string[] DownloadsOperatingSystems { get; set; }
        [DataMember(Name = "updateDownloads")]
        public string[] UpdateDownloads { get; set; }
        [DataMember(Name = "processDownloads")]
        public string[] ProcessDownloads { get; set; }
        [DataMember(Name = "validate")]
        public bool Validate { get; set; }
        [DataMember(Name = "cleanup")]
        public string[] Cleanup { get; set; }
        [DataMember(Name = "diagnosticsLog")]
        public bool DiagnosticsLog { get; set; }
    }
}
