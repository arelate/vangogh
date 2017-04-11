using System.Threading.Tasks;

namespace Interfaces.Hash
{
    public interface IGetHashDelegate<Input, Output>
    {
        Output GetHash(Input data);
    }

    public interface ISetHashDelegate<Input1, Input2>
    {
        Task SetHashAsync(Input1 data, Input2 hash);
    }

    public interface IBytesToStringHashController:
        IGetHashDelegate<byte[], string>
    {
        // ...
    }

    public interface IHashTrackingController:
        IGetHashDelegate<string, int>,
        ISetHashDelegate<string, int>
    {
        // ...
    }
}
