using System.Threading.Tasks;

using Interfaces.Delegates.Correct;

using Interfaces.Settings;
using Interfaces.Language;

namespace Delegates.Correct
{
    public class CorrectDownloadsOperatingSystemsAsyncDelegate : ICorrectAsyncDelegate<string[]>
    {
        private string[] defaultOperatingSystems = new string[1] { "Windows" };

        public async Task<string[]> CorrectAsync(string[] downloadsOperatingSystems)
        {
            return await Task.Run(() =>
            {
                if (downloadsOperatingSystems == null ||
                    downloadsOperatingSystems.Length == 0)
                    downloadsOperatingSystems = defaultOperatingSystems;

                return downloadsOperatingSystems;
            });
        }
    }
}
