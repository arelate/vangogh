package products

import (
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(product Product, mediaType urls.MediaType) error {
	return storage.Save(product, paths.Product(product.ID, mediaType))
}
