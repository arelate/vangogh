namespace Interfaces.ActivityContext
{
    public interface IIsWhitelistedDelegate
    {
        bool IsWhitelisted(ActivityDefinitions.Activity activity, ContextDefinitions.Context context);
    }

    public interface IParseDelegate
    {
        (ActivityDefinitions.Activity, ContextDefinitions.Context) Parse(string activityContext);
    }

    public interface IActivityContextController:
        IIsWhitelistedDelegate,
        IParseDelegate
    {
        // ...
    }
}
