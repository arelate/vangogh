using AC = System.ValueTuple<Interfaces.ActivityDefinitions.Activity, Interfaces.ContextDefinitions.Context>;

namespace Interfaces.ActivityContext
{
    public interface IIsAliasDelegate
    {
        bool IsAlias(string activityContext);
    }

    public interface IExpandAliasDelegate
    {
        AC[] ExpandAlias(string activityContext);
    }

    public interface IAliasController:
        IIsAliasDelegate,
        IExpandAliasDelegate
    {
        // ...
    }
}
