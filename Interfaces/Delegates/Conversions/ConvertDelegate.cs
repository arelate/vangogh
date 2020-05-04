namespace Interfaces.Delegates.Conversions
{
    public interface IConvertDelegate<in From, out To>
    {
        To Convert(From data);
    }
}