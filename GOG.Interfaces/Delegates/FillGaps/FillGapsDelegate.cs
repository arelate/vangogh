namespace GOG.Interfaces.Delegates.FillGaps
{
    // TODO: Refactor this to a more generic delegate or better update model
    public interface IFillGapsDelegate<FromType, ToType>
    {
        void FillGaps(FromType from, ToType to);
    }
}
