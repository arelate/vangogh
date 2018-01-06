namespace Interfaces.Delegates.Convert
{
    public interface IConvertDelegate<From, To>
    {
        To Convert(From data);
    }
}
