using System.Collections.Generic;

namespace Interfaces.ViewModel
{
    public interface IGetViewModelDelegate<T>
    {
        IDictionary<string, string> GetViewModel(T data);
    }
}
