using Interfaces.Delegates.Format;

namespace Delegates.Format.Uri
{
    public abstract class FormatUriDelegate : IFormatDelegate<string, string>
    {
        protected string uriTemplate;

        public string Format(string partialUri)
        {
            return string.Format(uriTemplate, partialUri);
        }
    }
}
