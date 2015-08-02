namespace GOG.Interfaces
{
    #region Product data provider

    public interface IProductDetailsProvider<Type>
    {
        string Message { get; }
        string RequestTemplate { get; }
        bool SkipCondition(Type element);
        string GetRequestDetails(Type element);
        void SetDetails(Type element, string data);
        IStringGetController StringGetController { get; }
    }

    #endregion
}
