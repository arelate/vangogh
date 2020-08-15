package details

import (
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(details *Details, id int, mt media.Type) error {
	return storage.Save(details, paths.Details(id, mt))
}
