using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Records;

using Interfaces.Models.RecordsTypes;

using Interfaces.Status;

namespace Controllers.Records.Session
{
    public class SessionRecordsController: StringRecordsController
    {
        public SessionRecordsController(
            IRecordsController<long> sessionIndexRecordsController,
            IConvertDelegate<string, long> convertStringToIndexDelegate):
            base(
                sessionIndexRecordsController,
                convertStringToIndexDelegate)
        {
            // ...
        }
    }
}
