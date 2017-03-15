using System.Collections.Generic;

using Interfaces.Data;

namespace Interfaces.Template
{
    public interface IPrimaryTemplateProperty
    {
        string PrimaryTemplate { get; }
    }

    public interface IResolveSubTemplatesDelegate
    {
        string ResolveSubTemplates(string templateContent);
    }

    public interface IResolveConditionalsDelegate
    {
        string ResolveConditionals(string templateContent, IDictionary<string, string> viewModel);
    }

    public interface IGetContentByTitleDelegate
    {
        string GetContentByTitle(string templateTitle);
    }

    public interface IBindDelegate
    {
        string Bind(string templateTitle, IDictionary<string, string> viewModel);
    }

    public interface ITemplateController:
        IPrimaryTemplateProperty, 
        IResolveSubTemplatesDelegate,
        IGetContentByTitleDelegate,
        IBindDelegate,
        ILoadDelegate
    {
        // ...
    }
}
