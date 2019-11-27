using System;
using System.Threading.Tasks;

using Interfaces.Delegates.Convert;

using Interfaces.Controllers.Records;

using Attributes;

namespace Controllers.Records.Session
{
    public class SessionRecordsController: StringRecordsController
    {
        [Dependencies(
            "Controllers.Records.Session.SessionRecordsIndexController,Controllers",
            "Delegates.Convert.ConvertStringToIndexDelegate,Delegates")]
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
