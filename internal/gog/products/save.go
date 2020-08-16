package products

import (
	"github.com/boggydigital/vangogh/internal/gog/index"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/jsonsha"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(p *Product, mt media.Type) error {
	bytes, sha, err := jsonsha.Marshal(p)
	if err != nil {
		return err
	}
	if index.Update(Indexes, p.ID, mt, sha) {
		return storage.Save(bytes, paths.Product(p.ID, mt))
	}
	return nil
}
