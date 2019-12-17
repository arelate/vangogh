using System.Collections.Generic;
using System.Threading.Tasks;

using Interfaces.Controllers.Data;
using Interfaces.Status;

namespace Interfaces.Template
{
    public interface IGetPrimaryTemplateTitleProperty
    {
        Task<string> GetPrimaryTemplateTitleAsync(IStatus status);
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
        IGetPrimaryTemplateTitleProperty, 
        IResolveSubTemplatesAsyncDelegate,
        IGetContentByTitleAsyncDelegate,
        IBindAsyncDelegate
    {
        // ...
    }
}
