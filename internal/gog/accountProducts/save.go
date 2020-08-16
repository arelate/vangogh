package accountProducts

import (
	"github.com/boggydigital/vangogh/internal/gog/index"
	media "github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/jsonsha"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(ap *AccountProduct, mt media.Type) error {
	bytes, sha, err := jsonsha.Marshal(ap)
	if err != nil {
		return err
	}
	if index.Update(Indexes, ap.ID, mt, sha) {
		return storage.Save(bytes, paths.AccountProduct(ap.ID, mt))
	}
	return nil
}
