using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Interfaces.Template;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.Collection;

using Interfaces.SerializedStorage;
using Interfaces.Status;

using Models.Separators;

namespace Controllers.Template
{
    public class TemplateController : ITemplateController
    {
        private ITemplate[] templates;
        private IGetDirectoryDelegate getDirectoryDelegate;
        private IGetFilenameDelegate getFilenameDelegate;
        private ISerializedStorageController serializedStorageController;
        private ICollectionController collectionController;

        private const string anyCharactersExpression = "(.*?)";
        private const string templateValuePrefix = Separators.TemplatePrefix;
        private const string templateValueSuffix = Separators.TemplateSuffix;
        private const string templateValue = templateValuePrefix + anyCharactersExpression + templateValueSuffix;
        private const string subTemplatePrefix = Separators.TemplatePrefix + Separators.TemplatePrefix;
        private const string subTemplateSuffix = Separators.TemplateSuffix + Separators.TemplateSuffix;
        private const string subTemplate = subTemplatePrefix + anyCharactersExpression + subTemplateSuffix;

        public TemplateController(
            string primaryTemplateTitle,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializedStorageController serializedStorageController,
            ICollectionController collectionController)
        {
            this.PrimaryTemplate = primaryTemplateTitle;
            this.getDirectoryDelegate = getDirectoryDelegate;
            this.getFilenameDelegate = getFilenameDelegate;
            this.serializedStorageController = serializedStorageController;
            this.collectionController = collectionController;
        }

        public string PrimaryTemplate { get; private set; }

        public bool DataAvailable
        {
            get;
            private set;
        }

        public async Task<string> BindAsync(string templateTitle, IDictionary<string, string> viewModel, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            var templateContent = await GetContentByTitleAsync(templateTitle, status);

            var resolvedConditionalsTemplateContent = await ResolveConditionalsAsync(templateContent, viewModel, status);
            var resolvedTemplateContent = await ResolveSubTemplatesAsync(resolvedConditionalsTemplateContent, status);

            var view = await FindAndReplace(
                resolvedTemplateContent,
                templateValue,
                templateValuePrefix,
                templateValueSuffix,
                async (property, replacementStatus) =>
                {
                    return await Task.Run(() =>
                    {
                        if (!viewModel.ContainsKey(property))
                            throw new InvalidDataException($"ViewModel doesn't contain {property} property");
                        return viewModel[property];
                    });
                },
                status);

            return view;
        }

        private delegate Task<string> GetContentAsync(string input, IStatus status);

        public async Task<string> GetContentByTitleAsync(string templateTitle, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            if (string.IsNullOrEmpty(templateTitle))
                return string.Empty;
            var template = collectionController.Find(templates, t => t.Title == templateTitle);
            return (template != null) ? template.Content : string.Empty;
        }

        public async Task LoadAsync(IStatus status = null)
        {
            var templateUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            templates = await serializedStorageController.DeserializePullAsync<Models.Template.Template[]>(templateUri, status);

            if (templates == null)
                templates = new Models.Template.Template[0];

            DataAvailable = true;
        }

        private async Task<string> FindAndReplace(string inputString,
            string regexPattern,
            string matchPrefix,
            string matchSuffix,
            GetContentAsync getReplacementContent,
            IStatus status)
        {
            var regex = new Regex(regexPattern);

            while (regex.IsMatch(inputString))
            {
                var match = regex.Match(inputString);
                var filteredMatch = match.Value.Substring(
                    matchPrefix.Length,
                    match.Length - matchPrefix.Length - matchSuffix.Length);
                var replacementContent = await getReplacementContent(filteredMatch, status);

                inputString = inputString.Replace(match.Value, replacementContent);
            }

            return inputString;
        }

        public async Task<string> ResolveSubTemplatesAsync(string templateContent, IStatus status)
        {
            if (!DataAvailable) await LoadAsync(status);

            return await FindAndReplace(
                templateContent,
                subTemplate,
                subTemplatePrefix,
                subTemplateSuffix,
                GetContentByTitleAsync,
                status);
        }

        public async Task<string> ResolveConditionalsAsync(string templateContent, IDictionary<string, string> viewModel, IStatus status)
        {
            return await FindAndReplace(
                templateContent,
                "<" + anyCharactersExpression + ">",
                "<",
                ">",
                async (match, replacementStatus) =>
                {
                    var conditionalParts = match.Split(new string[2] { "?", ":" }, StringSplitOptions.RemoveEmptyEntries);
                    if (conditionalParts.Length < 2)
                        throw new InvalidDataException("Conditional expression doesn't have expected separators");

                    var condition = conditionalParts[0];
                    var conditionPass = conditionalParts[1];
                    var conditionFail = string.Empty;
                    if (conditionalParts.Length >= 3)
                        conditionFail = conditionalParts[2];

                    return (viewModel.ContainsKey(condition) &&
                        !string.IsNullOrEmpty(viewModel[condition])) ?
                        await GetContentByTitleAsync(conditionPass, replacementStatus) :
                        await GetContentByTitleAsync(conditionFail, replacementStatus);
                },
                status);
        }
    }
}
