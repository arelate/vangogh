namespace Interfaces.UpdateDependencies
{
    public interface IConnectDelegate
    {
        void Connect<FromType, ToType>(FromType from, ToType to);
    }

    public interface IConnectionController:
        IConnectDelegate
    {
        // ...
    }
}
