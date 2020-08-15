package wishlist

import (
	"encoding/json"
	media "github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/products"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int, mt media.Type) (wp *products.Product, err error) {
	apBytes, err := storage.Load(paths.Wishlist(id, mt))

	if err != nil {
		return nil, err
	}

	err = json.Unmarshal(apBytes, &wp)

	return wp, err
}
