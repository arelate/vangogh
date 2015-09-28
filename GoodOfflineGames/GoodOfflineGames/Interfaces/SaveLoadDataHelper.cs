using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface ILoadDataHelper {
        Task<T> LoadData<T>(ProductTypes type);
    }

    public interface ISaveDataHelper
    {
        Task SaveData<T>(T data, ProductTypes type);
    }

    public interface ISaveLoadDataHelper:
        ISaveDataHelper,
        ILoadDataHelper
    {
        // ...
    }
}
