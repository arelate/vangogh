using Interfaces.Conversion;

namespace Controllers.Conversion
{
    public class JSONToJavaScriptConvetsionController : IConversionController<string, string>
    {
        private string prefix;

        public JSONToJavaScriptConvetsionController(string prefix)
        {
            this.prefix = prefix;
        }

        public string Convert(string data)
        {
            return (data.StartsWith(prefix)) ?
                data.Substring(prefix.Length) :
                prefix + data;
        }
    }
}
