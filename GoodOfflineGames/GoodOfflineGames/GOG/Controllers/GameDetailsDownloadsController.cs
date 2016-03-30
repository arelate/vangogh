using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG.Controllers
{
    public class GameDetailsDownloadsController : IGameDetailsDownloadsController
    {
        private const string downloadsStart = "[[";
        private const string downloadsEnd = "]]";
        private const string nullString = "null";

        public string ExtractSingle(string input)
        {
            // downloads are double array and so far nothing else in the game details data is
            // so we'll leverage this fact to extract actual content

            string result = string.Empty;

            int fromIndex = input.IndexOf(downloadsStart),
                toIndex = input.IndexOf(downloadsEnd);

            if (fromIndex < toIndex)
                result = input.Substring(
                    fromIndex, 
                    toIndex - fromIndex + downloadsEnd.Length);

            return result;
        }

        public string Sanitize(string input1, string input2)
        {
            return input1.Replace(input2, nullString);
        }
    }
}
