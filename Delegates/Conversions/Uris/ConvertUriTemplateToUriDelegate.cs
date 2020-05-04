using Interfaces.Delegates.Conversions;

namespace Delegates.Conversions.Uris
{
    public abstract class ConvertUriTemplateToUriDelegate : IConvertDelegate<string, string>
    {
        protected string uriTemplate;

        public string Convert(string partialUri)
        {
            return string.Format(uriTemplate, partialUri);
        }
    }
}