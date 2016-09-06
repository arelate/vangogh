namespace Interfaces.UpdateDependencies
{
    public interface IConnectDelegate
    {
        FromType Connect<FromType, ToType>(FromType from, ToType to);
    }

    public interface IConnectionController:
        IConnectDelegate
    {
        // ...
    }
}
