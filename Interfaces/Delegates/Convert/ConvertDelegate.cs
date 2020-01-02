using System.Threading.Tasks;

namespace Interfaces.Delegates.Convert
{
    public interface IConvertDelegate<From, To>
    {
        To Convert(From data);
    }

    public interface IConvertAsyncDelegate<From, To>
    {
        To ConvertAsync(From data);
    }

}
