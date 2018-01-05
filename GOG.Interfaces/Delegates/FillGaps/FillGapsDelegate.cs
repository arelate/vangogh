namespace GOG.Interfaces.Delegates.FillGaps
{
    public interface IFillGapsDelegate<FromType, ToType>
    {
        void FillGaps(FromType from, ToType to);
    }
}
