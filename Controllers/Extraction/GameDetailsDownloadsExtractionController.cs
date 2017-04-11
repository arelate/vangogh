using System.Collections.Generic;

using Interfaces.Extraction;

using Models.Separators;

namespace Controllers.Extraction
{
    public class GameDetailsDownloadsExtractionController : IStringExtractionController
    {
        public IEnumerable<string> ExtractMultiple(string data)
        {
            // downloads are double array and so far nothing else in the game details data is
            // so we'll leverage this fact to extract actual content

            string result = string.Empty;

            int fromIndex = data.IndexOf(Separators.GameDetailsDownloadsStart),
                toIndex = data.IndexOf(Separators.GameDetailsDownloadsEnd);

            if (fromIndex < toIndex)
                result = data.Substring(
                    fromIndex,
                    toIndex - fromIndex + Separators.GameDetailsDownloadsEnd.Length);

            return new string[] { result };
        }
    }
}
