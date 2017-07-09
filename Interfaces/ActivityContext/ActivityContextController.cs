using System.Collections.Generic;

namespace Interfaces.ActivityContext
{
    public interface IIsWhitelistedDelegate
    {
        bool IsWhitelisted((ActivityDefinitions.Activity Activity, ContextDefinitions.Context Context) activityContext);
    }

    public interface IParseSingleDelegate
    {
        (ActivityDefinitions.Activity, ContextDefinitions.Context) ParseSingle(string activityContext);
    }

    public interface ICreateActivityContextQueueDelegate
    {
        IEnumerable<(ActivityDefinitions.Activity, ContextDefinitions.Context)> CreateActivityContextQueue(string[] args);
    }

    public interface IGetParametersDelegate
    {
        IEnumerable<string> GetParameters(string[] args);
    }

    public interface IActivityContextController:
        IIsWhitelistedDelegate,
        IParseSingleDelegate,
        ICreateActivityContextQueueDelegate,
        IGetParametersDelegate
    {
        // ...
    }
}
