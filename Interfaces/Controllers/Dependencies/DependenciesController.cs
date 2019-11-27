using System;
using System.Reflection;
using System.Collections.Generic;

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

    public interface IGetTypeDependencyGraphDelegate
    {
        List<(Type, int)> GetTypeDependencyGraph(Type type, int level = 0);
    }

    public interface IDependencyGraphToStringDelegate
    {
        IEnumerable<string> DependencyGraphToString(List<(Type, int)> dependencyGraph);
    }

    public interface IDependenciesController :
        IInstantiateTypeDelegate,
        IInstantiateTypesDelegate,
        IGetDependentConstructorDelegate,
        IGetDependentConstructorDependencyTypesDelegate,
        IGetTypeDependencyGraphDelegate,
        IDependencyGraphToStringDelegate        
    {
        // ...
    }
}