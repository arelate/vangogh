using System;

namespace Interfaces.Controllers.Dependencies
{
    public interface IGetInstanceDelegate
    {
        object GetInstance(Type type);
    }

    public interface IAddDependencyDelegate
    {
        void AddDependencies<TargetType>(params Type[] dependencies) where TargetType : class;
    }

    public interface IDependenciesController :
        IAddDependencyDelegate,
        IGetInstanceDelegate
    {
        // ...
    }
}