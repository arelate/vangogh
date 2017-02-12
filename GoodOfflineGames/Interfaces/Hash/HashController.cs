namespace Interfaces.Hash
{
    public interface IComputeHashDelegate<Input, Output>
    {
        Output ComputeHash(Input data);
    }

    public interface IHashController<Input, Output>:
        IComputeHashDelegate<Input, Output>
    {
        // ...
    }

    public interface IBytesToStringHashController:
        IHashController<byte[], string>
    {
        // ...
    }
}
