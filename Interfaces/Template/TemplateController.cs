using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Data;
using Interfaces.Status;

namespace Interfaces.Template
{
    public interface IPrimaryTemplateProperty
    {
        string PrimaryTemplate { get; }
    }

    public interface IResolveSubTemplatesAsyncDelegate
    {
        Task<string> ResolveSubTemplatesAsync(string templateContent, IStatus status);
    }

    public interface IResolveConditionalsAsyncDelegate
    {
        Task<string> ResolveConditionalsAsync(string templateContent, IDictionary<string, string> viewModel, IStatus status);
    }

    public interface IGetContentByTitleAsyncDelegate
    {
        Task<string> GetContentByTitleAsync(string templateTitle, IStatus status);
    }

    public interface IBindAsyncDelegate
    {
        Task<string> BindAsync(string templateTitle, IDictionary<string, string> viewModel, IStatus status);
    }

    public interface ITemplateController:
        IPrimaryTemplateProperty, 
        IResolveSubTemplatesAsyncDelegate,
        IGetContentByTitleAsyncDelegate,
        IBindAsyncDelegate,
        ILoadAsyncDelegate,
        IDataAvailableDelegate
    {
        // ...
    }
}
