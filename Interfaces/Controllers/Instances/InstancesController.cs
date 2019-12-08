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

    public interface IGetInstantiationConstructorInfoDelegate
    {
        ConstructorInfo GetInstantiationConstructorInfo(Type type);
    }

    public interface IGetTypesForConstructorDelegate
    {
        Type[] GetTypesForConstructor(ConstructorInfo constructorInfo);
    }

    public interface IInstancesController :
        IGetInstanceDelegate,
        IGetInstancesDelegate,
        IGetInstantiationConstructorInfoDelegate,
        IGetTypesForConstructorDelegate
    {
        // ...
    }
}