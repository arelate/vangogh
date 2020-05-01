using System.Collections.Generic;

namespace Models.Requests
{
    public class Request
    {
        public string Method { get; set; }
        public string Collection { get; set; }
        public IDictionary<string, IEnumerable<string>> Parameters { get; set; }
    }
}