using System;
using System.Reflection;

namespace Interfaces.Controllers.Dependencies
{
    public interface IInstantiateTypeDelegate
    {
        object Instantiate(Type type);
    }

    public interface IInstantiateTypesDelegate
    {
        object[] Instantiate(Type[] types);
    }

    public interface IGetDependentConstructorDelegate
    {
        ConstructorInfo GetDependentConstructor(Type type);
    }

    public interface IGetDependentConstructorDependencyTypesDelegate
    {
        Type[] GetDependentConstructorDependencyTypes(ConstructorInfo constructorInfo);
    }

    public interface IDependenciesController :
        IInstantiateTypeDelegate,
        IInstantiateTypesDelegate,
        IGetDependentConstructorDelegate,
        IGetDependentConstructorDependencyTypesDelegate
    {
        // ...
    }
}