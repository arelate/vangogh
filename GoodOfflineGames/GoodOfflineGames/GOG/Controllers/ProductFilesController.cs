using System.Collections.Generic;
using System.Linq;

using GOG.Model;
using GOG.Interfaces;

namespace GOG.Controllers
{
    class ProductFilesController: IProductFileController
    {
        private IList<ProductFile> productFiles;

        public ProductFilesController(IList<ProductFile> productFiles)
        {
            this.productFiles = productFiles;
        }

        public bool CheckSuccess()
        {
            var productFilesDownloadedSuccessfully = true;

            foreach (var productFile in productFiles)
                productFilesDownloadedSuccessfully &=
                    productFile.DownloadSuccessful;

            return productFilesDownloadedSuccessfully;
        }

        private delegate T GetValueDelegate<T>(ProductFile productFile);

        private IEnumerable<T> GetUniqueValues<T>(GetValueDelegate<T> getValueDelegate)
        {
            var processedValues = new List<T>();
            foreach (var productFile in productFiles)
            {
                var value = getValueDelegate(productFile);

                if (processedValues.Contains(value))
                    continue;

                processedValues.Add(value);
                yield return value;
            }
        }

        public IEnumerable<long> GetIds()
        {
            return GetUniqueValues(pf => pf.Id);
        }

        public IEnumerable<string> GetFolders()
        {
            return GetUniqueValues(pf => pf.Folder);
        }

        public IEnumerable<string> GetFiles(string folder)
        {
            foreach (var productFile in productFiles)
            {
                if (productFile.Folder == folder)
                    yield return productFile.File;
            }
        }

        public IList<ProductFile> Filter(IList<ProductFile> inputProductFiles)
        {
            IList<ProductFile> outputProductFiles = new List<ProductFile>();
            var ids = GetIds();

            foreach (var productFile in inputProductFiles)
            {
                if (!ids.Contains(productFile.Id))
                    outputProductFiles.Add(productFile);
            }

            return outputProductFiles;
        }
    }
}
