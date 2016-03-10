using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface ILoadDelegate<Type>
    {
        Task<Type> Load();
    }

    public interface ISaveDelegate<Type>
    {
        Task Save(Type data);
    }

    public interface ISettingsController<Type>:
        ILoadDelegate<Type>,
        ISaveDelegate<Type>
    {
        // ...
    }
}
