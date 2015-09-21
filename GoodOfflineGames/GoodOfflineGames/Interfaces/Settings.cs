using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GOG.Interfaces
{
    public interface ILoadSettingsDelegate<Type>
    {
        Task<Type> Load();
    }

    public interface ISaveSettingsDelegate<Type>
    {
        Task Save(Type data);
    }

    public interface ISettingsController<Type>:
        ILoadSettingsDelegate<Type>,
        ISaveSettingsDelegate<Type>
    {
        // ...
    }
}
