namespace Interfaces.Delegates.Conversions
{
    public interface IConvertAsyncDelegate<in From, out To>
    {
        public To ConvertAsync(From data);
    }
}