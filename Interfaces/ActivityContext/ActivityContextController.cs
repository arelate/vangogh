using System.Collections.Generic;

using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.Models.Entities.Entity>;

namespace Interfaces.ActivityContext
{
    public interface IParseSingleDelegate
    {
        AC ParseSingle(string activityContext);
    }

    public interface IToStringDelegate
    {
        string ToString(AC activityContext);
    }

    public interface IGetQueueDelegate
    {
        IEnumerable<AC> GetQueue(string[] args);
    }

    public interface IGetParametersDelegate
    {
        IEnumerable<string> GetParameters(string[] args);
    }

    public interface IActivityContextController:
        IParseSingleDelegate,
        IToStringDelegate,
        IGetQueueDelegate,
        IGetParametersDelegate
    {
        // ...
    }
}
