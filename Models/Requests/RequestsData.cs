using System.Collections.Generic;

namespace Models.Requests
{
    public class RequestsData
    {
        public List<string> Methods { get; set; } = new List<string>();
        public List<string> Collections { get; set; } = new List<string>();
        public Dictionary<string, List<string>> Parameters { get; set; } = new Dictionary<string, List<string>>();
        public List<string> UnknownTokens { get; set; } = new List<string>();
    }
}
