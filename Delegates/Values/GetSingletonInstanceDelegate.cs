using Interfaces.Delegates.Values;

namespace Delegates.Values
{
    public abstract class GetSingletonInstanceDelegate<T> : IGetInstanceDelegate<T>
        where T : class, new()
    {
        private readonly T value;

        protected GetSingletonInstanceDelegate()
        {
            value = new T();
        }

        public T GetInstance()
        {
            return value;
        }
    }
}