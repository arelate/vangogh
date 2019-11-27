using System.Collections.Generic;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Collection;

using Attributes;

namespace Controllers.Template.App
{
    public class AppTemplateController: TemplateController
    {
        [Dependencies(
            "Controllers.Stash.Templates.AppTemplateStashController,Controllers",
            "Controllers.Collection.CollectionController,Controllers")]
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