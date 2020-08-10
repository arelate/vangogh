package products

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(product Product, mediaType string) error {
	return storage.Save(product, paths.Product(product.ID, mediaType))
}

func Load(id int, mediaType string) (product *Product, err error) {
	pBytes, err := storage.Load(paths.Product(id, mediaType))

	if err != nil {
		return product, err
	}

	err = json.Unmarshal(pBytes, &product)

	return product, err
}
