using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interfaces.Template
{
    public interface IGetPrimaryTemplateTitleProperty
    {
        Task<string> GetPrimaryTemplateTitleAsync();
    }

    public interface IResolveSubTemplatesAsyncDelegate
    {
        Task<string> ResolveSubTemplatesAsync(string templateContent);
    }

    public interface IResolveConditionalsAsyncDelegate
    {
        Task<string> ResolveConditionalsAsync(string templateContent, IDictionary<string, string> viewModel);
    }

    public interface IGetContentByTitleAsyncDelegate
    {
        Task<string> GetContentByTitleAsync(string templateTitle);
    }

    public interface IBindAsyncDelegate
    {
        Task<string> BindAsync(string templateTitle, IDictionary<string, string> viewModel);
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
