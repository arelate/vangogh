using System.Threading.Tasks;

namespace Interfaces.Settings
{
    public interface ILoadDelegate<Type>
    {
        Task<Type> Load();
    }

    //public interface ISaveDelegate<Type>
    //{
    //    Task Save(Type data);
    //}

    public interface ISettingsController<Type>:
        ILoadDelegate<Type>
    {
        // ...
    }
}
