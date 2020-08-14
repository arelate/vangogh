package products

import (
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(p *Product, mt urls.MediaType) error {
	return storage.Save(p, paths.Product(p.ID, mt))
}
