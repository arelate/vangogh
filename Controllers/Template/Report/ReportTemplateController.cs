using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

namespace Controllers.Template.Report
{
    public class ReportTemplateController: TemplateController
    {
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