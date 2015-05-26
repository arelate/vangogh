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
        protected IStringRequestController stringRequestController;
        protected ISerializationController serializationController;

        public AbstractDetailsProvider(
            IStringRequestController stringRequestController,
            ISerializationController serializationController)
        {
            this.stringRequestController = stringRequestController;
            this.serializationController = serializationController;
        }

    }
}
