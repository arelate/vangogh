package wishlist

import (
	media "github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/products"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(wp *products.Product, mt media.Type) error {
	return storage.Save(wp, paths.Wishlist(wp.ID, mt))
}
