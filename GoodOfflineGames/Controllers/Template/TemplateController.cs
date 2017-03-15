using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Interfaces.Template;
using Interfaces.Destination.Directory;
using Interfaces.Destination.Filename;
using Interfaces.SerializedStorage;
using Interfaces.Collection;

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

        public string Bind(string templateTitle, IDictionary<string, string> viewModel)
        {
            var templateContent = GetContentByTitle(templateTitle);
            var resolvedConditionalsTemplateContent = ResolveConditionals(templateContent, viewModel);
            var resolvedTemplateContent = ResolveSubTemplates(resolvedConditionalsTemplateContent);

            var view = FindAndReplace(
                resolvedTemplateContent,
                templateValue,
                templateValuePrefix,
                templateValueSuffix,
                property =>
                {
                    if (!viewModel.ContainsKey(property))
                        throw new InvalidDataException($"ViewModel doesn't contain {property} property");
                    return viewModel[property];
                });

            return view;
        }

        public string GetContentByTitle(string templateTitle)
        {
            if (string.IsNullOrEmpty(templateTitle))
                return string.Empty;
            var template = collectionController.Find(templates, t => t.Title == templateTitle);
            return (template != null) ? template.Content : string.Empty;
        }

        public async Task LoadAsync()
        {
            var templateUri = Path.Combine(
                getDirectoryDelegate.GetDirectory(),
                getFilenameDelegate.GetFilename());

            templates = await serializedStorageController.DeserializePullAsync<Models.Template.Template[]>(templateUri);

            if (templates == null)
                templates = new Models.Template.Template[0];
        }

        private string FindAndReplace(string inputString,
            string regexPattern,
            string matchPrefix,
            string matchSuffix,
            Func<string, string> getReplacementContent)
        {
            var regex = new Regex(regexPattern);

            while (regex.IsMatch(inputString))
            {
                var match = regex.Match(inputString);
                var filteredMatch = match.Value.Substring(
                    matchPrefix.Length,
                    match.Length - matchPrefix.Length - matchSuffix.Length);
                var replacementContent = getReplacementContent(filteredMatch);

                inputString = inputString.Replace(match.Value, replacementContent);
            }

            return inputString;
        }

        public string ResolveSubTemplates(string templateContent)
        {
            return FindAndReplace(
                templateContent,
                subTemplate,
                subTemplatePrefix,
                subTemplateSuffix,
                GetContentByTitle);
        }

        public string ResolveConditionals(string templateContent, IDictionary<string, string> viewModel)
        {
            return FindAndReplace(
                templateContent,
                "<" + anyCharactersExpression + ">",
                "<",
                ">",
                match =>
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
                        GetContentByTitle(conditionPass) :
                        GetContentByTitle(conditionFail);
                });
        }
    }
}
