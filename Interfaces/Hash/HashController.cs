﻿using System.Threading.Tasks;

namespace Interfaces.Hash
{
    public interface IGetHashDelegate<Type>
    {
        string GetHash(Type data);
    }

    public interface ISetHashAsyncDelegate<Type>
    {
        Task SetHashAsync(Type data, string hash);
    }

    public interface IBytesHashController:
        IGetHashDelegate<byte[]>
    {
        // ...
    }

    public interface IStringHashController:
        IGetHashDelegate<string>
    {
        // ...
    }

    public interface IPrecomputedHashController:
        IGetHashDelegate<string>,
        ISetHashAsyncDelegate<string>
    {
        // ...
    }
}