using Models.Extensions;

namespace Delegates.Values.Filenames
{
    public class GetValidationFilenameDelegate : GetUriFilenameDelegate
    {
        public override string GetValue(string source = null)
        {
            var filename = base.GetValue(source);
            if (!filename.EndsWith(Extensions.XML, System.StringComparison.Ordinal))
                filename += Extensions.XML;

            return filename;
        }
    }
}