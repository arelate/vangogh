using Interfaces.Delegates.Format;

namespace Delegates.Format
{
    public abstract class FormatUriDelegate : IFormatDelegate<string, string>
    {
        protected string uriTemplate;

        public virtual string Format(string partialUri)
        {
            return string.Format(uriTemplate, partialUri); ;
        }
    }
}
