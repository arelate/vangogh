using Interfaces.Delegates.GetValue;

namespace Delegates.GetValue
{
    public abstract class GetObjectValueDelegate<T> : IGetValueDelegate<T>
        where T : class, new()
    {
        private readonly T value;

        public GetObjectValueDelegate()
        {
            value = new T();
        }

        public T GetValue()
        {
            return value;
        }
    }
}