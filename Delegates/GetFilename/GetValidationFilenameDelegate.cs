using Models.Uris;

namespace Delegates.GetFilename
{
    public class GetValidationFilenameDelegate : GetUriFilenameDelegate
    {
        public override string GetFilename(string source = null)
        {
            var filename = base.GetFilename(source);
            if (!filename.EndsWith(Uris.Extensions.Validation.ValidationExtension))
                filename += Uris.Extensions.Validation.ValidationExtension;

            return filename;
        }
    }
}
