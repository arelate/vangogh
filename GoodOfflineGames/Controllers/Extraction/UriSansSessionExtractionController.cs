using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;

using Interfaces.Extraction;

using Models.Separators;

namespace Controllers.Extraction
{
    public class UriSansSessionExtractionController : IStringExtractionController
    {
        public IEnumerable<string> ExtractMultiple(string uri)
        {
            if (string.IsNullOrEmpty(uri)) return new string[0];

            var uriParts = uri.Split(new string[] { Separators.QueryString }, StringSplitOptions.RemoveEmptyEntries);

            if (uriParts == null) return new string[0];
            if (uriParts.Length < 1) return new string[0];

            return new string[1] { uriParts[0] };
        }
    }
}
