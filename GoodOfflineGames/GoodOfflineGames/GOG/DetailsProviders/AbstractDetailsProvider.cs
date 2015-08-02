using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GOG.Interfaces;

namespace GOG.Providers
{
    public abstract class AbstractDetailsProvider
    {
        protected IStringGetController stringGetController;
        protected IStringifyController serializationController;

        public AbstractDetailsProvider(
            IStringGetController stringGetController,
            IStringifyController serializationController)
        {
            this.stringGetController = stringGetController;
            this.serializationController = serializationController;
        }

    }
}
