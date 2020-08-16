package details

import (
	"github.com/boggydigital/vangogh/internal/gog/index"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/jsonsha"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(d *Details, id int, mt media.Type) error {
	bytes, sha, err := jsonsha.Marshal(d)
	if err != nil {
		return err
	}
	if index.Update(Indexes, id, mt, sha) {
		return storage.Save(bytes, paths.Details(id, mt))
	}
	return nil
}
