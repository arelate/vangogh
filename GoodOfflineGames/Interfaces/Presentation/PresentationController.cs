using System.Collections.Generic;

namespace Interfaces.Presentation
{
    public interface IPresentDelegate<T>
    {
        void Present(IEnumerable<T> views, bool overrideThrottling = false);
    }

    public interface IPresentationController<T> :
        IPresentDelegate<T>
    {
        // ...
    }
}
