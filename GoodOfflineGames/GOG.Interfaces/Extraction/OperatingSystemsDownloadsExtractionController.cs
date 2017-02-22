using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Interfaces.Extraction;

using GOG.Models;

namespace GOG.Interfaces.Extraction
{
    public interface ILanguagesProperty
    {
        IEnumerable<string> Languages { get; set; }
    }

    public interface IOperatingSystemsDownloadsExtractionController: 
        IExtractMultipleDelegate<OperatingSystemsDownloads[][], OperatingSystemsDownloads>,
        ILanguagesProperty
    {
        // ...
    }
}
