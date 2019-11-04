using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

namespace Controllers.Template.App
{
    public class AppTemplateController: TemplateController
    {
        public AppTemplateController(
            IStashController<List<Models.Template.Template>> appTemplateStashController,
            ICollectionController collectionController):
            base(
                appTemplateStashController,
                collectionController)
        {
            // ...
        }
    }
}