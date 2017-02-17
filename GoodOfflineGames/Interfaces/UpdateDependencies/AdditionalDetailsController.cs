namespace Interfaces.UpdateDependencies
{
    public interface IAddDetailsDelegate
    {
        void AddDetails<Type>(Type element, string data);
    }

    public interface IAdditionalDetailsController:
        IAddDetailsDelegate
    {
        // ...
    }
}
