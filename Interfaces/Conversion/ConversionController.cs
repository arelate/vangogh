namespace Interfaces.Conversion
{
    public interface IConvertDelegate<From, To>
    {
        To Convert(From data);
    }

    public interface IConversionController<From, To>:
        IConvertDelegate<From, To>
    {
        // ...
    }
}
