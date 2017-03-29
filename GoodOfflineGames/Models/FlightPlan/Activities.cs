using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.FlightPlan
{
    public static class Activities
    {
        public const string UpdateData = "updateData";
        public const string UpdateDownloads = "updateDownloads";
        public const string ProcessDownloads = "processDownloads";
        public const string Validate = "validate";
        public const string Cleanup = "cleanup";
        public const string Report = "report";
    }
}
