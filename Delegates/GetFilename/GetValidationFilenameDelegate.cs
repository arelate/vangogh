using Models.Extensions;

namespace Delegates.GetFilename
{
    public class GetValidationFilenameDelegate : GetUriFilenameDelegate
    {
        public override string GetFilename(string source = null)
        {
            var filename = base.GetFilename(source);
            if (!filename.EndsWith(Extensions.XML, System.StringComparison.Ordinal))
                filename += Extensions.XML;

            return filename;
        }
    }
}
