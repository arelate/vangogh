package products

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(p *Product, mt media.Type) error {
	return storage.Save(p, paths.Product(p.ID, mt))
}
