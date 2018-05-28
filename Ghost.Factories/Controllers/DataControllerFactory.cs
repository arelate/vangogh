using System;
using System.Collections.Generic;

using Interfaces.Delegates.Convert;
using Interfaces.Delegates.GetDirectory;
using Interfaces.Delegates.GetFilename;
using Interfaces.Delegates.GetPath;
using Interfaces.Delegates.Recycle;


using Interfaces.Controllers.Serialization;
using Interfaces.Controllers.Storage;
using Interfaces.Controllers.SerializedStorage;
using Interfaces.Controllers.Collection;
using Interfaces.Controllers.Stash;
using Interfaces.Controllers.Index;
using Interfaces.Controllers.Data;
using Interfaces.Controllers.Records;

using Interfaces.Models.Entities;

using Interfaces.Status;

using Delegates.Convert;
using Delegates.GetDirectory;
using Delegates.GetFilename;
using Delegates.GetPath;

using Controllers.Stash;
using Controllers.Index;
using Controllers.Data;
using Controllers.Records;

using Models.ProductCore;
using Models.Directories;
using Models.Filenames;
using Models.ProductRecords;

using GOG.Models;

namespace Ghost.Factories.Controllers
{
    public class DataControllerFactory
    {
        private readonly ICollectionController collectionController;
        private readonly ISerializationController<string> serializationController;
        private readonly IStorageController<string> storageController;
        private ISerializedStorageController serializedStorageController;
        private IRecycleDelegate recycleDelegate;
        private IStatusController statusController;

        private IGetDirectoryDelegate getDataDirectoryDelegate;
        private IGetFilenameDelegate getJsonFilenameDelegate;

        private IDictionary<Entity, IIndexController<long>> indexControllers;
        private IDictionary<Entity, IStashController<List<long>>> indexStashControllers;

        private IDictionary<Entity, IGetDirectoryDelegate> getIndexDirectoryDelegates;
        private IDictionary<Entity, IGetPathDelegate> getIndexPathDelegates;

        private IDictionary<Type, Entity> typeToEntityMapping;

        private IGetFilenameDelegate getIndexFilenameDelegate;

        public DataControllerFactory(
            ICollectionController collectionController,
            ISerializationController<string> serializationController,
            IStorageController<string> storageController,
            ISerializedStorageController serializedStorageController,
            IRecycleDelegate recycleDelegate,
            IGetDirectoryDelegate getDataDirectoryDelegate,
            IGetFilenameDelegate getJsonFilenameDelegate,
            IStatusController statusController)
        {
            this.collectionController = collectionController;
            this.serializationController = serializationController;
            this.storageController = storageController;
            this.serializedStorageController = serializedStorageController;
            this.recycleDelegate = recycleDelegate;
            this.statusController = statusController;

            this.getDataDirectoryDelegate = getDataDirectoryDelegate;
            this.getJsonFilenameDelegate = getJsonFilenameDelegate;

            var dataEntitiesCount = Directories.Data.Count;

            indexControllers = new Dictionary<Entity, IIndexController<long>>(dataEntitiesCount);
            indexStashControllers = new Dictionary<Entity, IStashController<List<long>>>(dataEntitiesCount);

            getIndexDirectoryDelegates = new Dictionary<Entity, IGetDirectoryDelegate>(dataEntitiesCount);
            getIndexPathDelegates = new Dictionary<Entity, IGetPathDelegate>(dataEntitiesCount);

            getIndexFilenameDelegate = new GetFixedFilenameDelegate(Filenames.Base[Entity.Index], getJsonFilenameDelegate);

            typeToEntityMapping = new Dictionary<Type, Entity>
            {
                { typeof(Product), Entity.Products },
                { typeof(AccountProduct), Entity.AccountProducts }
            };
        }

        public Entity GetEntityFromType<Type>() where Type: ProductCore
        {
            return typeToEntityMapping[typeof(Type)];
        }

        public IGetDirectoryDelegate GetDirectoryDelegate(Entity entity)
        {
            if (!getIndexDirectoryDelegates.ContainsKey(entity))
                getIndexDirectoryDelegates.Add(
                    entity, 
                    new GetRelativeDirectoryDelegate(
                        Directories.Data[entity], 
                        getDataDirectoryDelegate));

            return getIndexDirectoryDelegates[entity];
        }

        public IGetPathDelegate GetIndexPathDelegate(Entity entity)
        {
            return new GetPathDelegate(
                GetDirectoryDelegate(entity),
                getIndexFilenameDelegate);
        }

        public IGetPathDelegate GetDataPathDelegate(Entity entity)
        {
            return new GetPathDelegate(
                GetDirectoryDelegate(entity),
                getJsonFilenameDelegate);
        }

        public IRecordsController<long> GetRecordsController()
        {
            throw new NotImplementedException();
        }

        public IStashController<List<long>> GetIndexStashController(Entity entity)
        {
            if (!indexStashControllers.ContainsKey(entity))
                indexStashControllers.Add(
                    entity,
                    new StashController<List<long>>(
                        GetIndexPathDelegate(entity),
                        serializationController,
                        storageController,
                        statusController));
            
            return indexStashControllers[entity];
        }

        public IStashController<List<Type>> GetDataStashController<Type>(Entity entity) where Type: ProductCore
        {
            return new StashController<List<Type>>(
                GetDataPathDelegate(entity),
                serializationController,
                storageController,
                statusController);
        }

        public IIndexController<long> GetIndexController(Entity entity)
        {
            var type = typeof(Type);
            if (!indexControllers.ContainsKey(entity))
                indexControllers.Add(
                    entity, 
                    new IndexController<long>(
                        GetIndexStashController(entity),
                        collectionController,
                        null,
                        statusController));

            return indexControllers[entity];
        }

        public IConvertDelegate<Type, long> GetConvertToIndexDelegate<Type>() where Type: ProductCore
        {
            return new ConvertProductCoreToIndexDelegate<Type>();
        }

        public IDataController<Type> GetDataController<Type>() where Type: ProductCore
        {
            var entity = GetEntityFromType<Type>();

            return new DataController<Type>(
                GetIndexController(entity),
                serializedStorageController,
                GetConvertToIndexDelegate<Type>(),
                collectionController,
                GetDataPathDelegate(entity),
                recycleDelegate,
                null,
                statusController);
        }
    }
}
