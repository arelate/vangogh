namespace Interfaces.Connection
{
    public interface IConnectDelegate<FromType, ToType>
    {
        void Connect(FromType from, ToType to);
    }
}
