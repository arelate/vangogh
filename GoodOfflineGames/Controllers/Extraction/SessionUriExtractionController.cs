using System;
using System.Collections.Generic;

using Interfaces.Extraction;

using Models.Separators;

namespace Controllers.Extraction
{
    public class SessionUriExtractionController : IStringExtractionController
    {
        public IEnumerable<string> ExtractMultiple(string sessionUri)
        {
            if (string.IsNullOrEmpty(sessionUri)) return new string[] { sessionUri };
            return sessionUri.Split(
                new string[] {
                    Separators.QueryString,
                    Separators.QueryStringParameters },
                StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
