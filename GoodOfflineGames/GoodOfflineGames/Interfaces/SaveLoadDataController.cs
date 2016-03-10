using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface ILoadDataDelegate {
        Task<T> LoadData<T>(ProductTypes type);
    }

    public interface ISaveDataDelegate
    {
        Task SaveData<T>(T data, ProductTypes type);
    }

    public interface ISaveLoadDataController:
        ILoadDataDelegate,
        ISaveDataDelegate
    {
        // ...
    }
}
