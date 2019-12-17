using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Attributes;

namespace Controllers.Template.Report
{
    public class ReportTemplateController: TemplateController
    {
        [Dependencies(
            "Controllers.Stash.Templates.ReportTemplateStashController,Controllers",
            "Controllers.Collection.CollectionController,Controllers")]
        public ReportTemplateController(
            IStashController<List<Models.Template.Template>> reportTemplateStashController,
            ICollectionController collectionController):
            base(
                reportTemplateStashController,
                collectionController)
        {
            // ...
        }
    }
}