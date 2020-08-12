package products

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int, mediaType urls.MediaType) (product *Product, err error) {
	pBytes, err := storage.Load(paths.Product(id, mediaType))

	if err != nil {
		return product, err
	}

	err = json.Unmarshal(pBytes, &product)

	return product, err
}
