using System;
using System.Reflection;

namespace Interfaces.Controllers.Instances
{

    public interface IGetInstanceDelegate
    {
        object GetInstance(Type type);
    }

    public interface IGetInstancesDelegate
    {
        object[] GetInstances(Type[] types);
    }

    public interface IGetDependentConstructorDelegate
    {
        ConstructorInfo GetDependentConstructor(Type type);
    }

    public interface IGetDependentConstructorDependencyTypesDelegate
    {
        Type[] GetDependentConstructorDependencyTypes(ConstructorInfo constructorInfo);
    }

    public interface IInstancesController :
        IGetInstanceDelegate,
        IGetInstancesDelegate,
        IGetDependentConstructorDelegate,
        IGetDependentConstructorDependencyTypesDelegate
    {
        // ...
    }
}