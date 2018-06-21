using System;
using System.Collections.Generic;
using System.Text;

using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;

using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Storage;

using Interfaces.Status;

using Interfaces.Models.Entities;

using Delegates.GetPath;

using Controllers.Stash;

using Ghost.Factories.Delegates;

namespace Ghost.Factories.Controllers
{
    public static class StashControllerFactory
    {
        public static IStashController<List<long>> CreateStashController(
            Entity entity,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            IStatusController statusController)
        {
            return new StashController<List<long>>(
                new GetPathDelegate(
                    getDirectoryDelegate,
                    getFilenameDelegate),
                serializationController,
                storageController,
                statusController);
        }

        public static IStashController<Dictionary<long, Type>> CreateDataStashController<Type>(
            Entity entity,
            IGetDirectoryDelegate getDirectoryDelegate,
            IGetFilenameDelegate getFilenameDelegate,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            IStatusController statusController)
        {
            return new StashController<Dictionary<long, Type>>(
                new GetPathDelegate(
                    getDirectoryDelegate,
                    getFilenameDelegate),
                serializationController,
                storageController,
                statusController);
        }
    }
}
