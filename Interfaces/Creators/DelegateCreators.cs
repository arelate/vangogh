namespace Interfaces.Creators
{
    public interface IDelegateCreator<DelegateType>
    {
        DelegateType CreateDelegate();
    }
}