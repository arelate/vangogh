using System.Threading.Tasks;

namespace Interfaces.Delegates.Convert
{
    public interface IConvertDelegate<From, To>
    {
        To Convert(From data);
    }

    public interface IConvertAsyncDelegate<From, To>
    {
        public To ConvertAsync(From data);
    }

}
