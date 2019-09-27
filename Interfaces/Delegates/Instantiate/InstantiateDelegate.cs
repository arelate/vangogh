using System;

namespace Interfaces.Delegates.Instantiate
{
    public interface IInstantiateDelegate
    {
        object Instantiate(Type type);
    }
}