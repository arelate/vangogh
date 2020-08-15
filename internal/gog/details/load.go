package details

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int, mt media.Type) (details *Details, err error) {
	dBytes, err := storage.Load(paths.Details(id, mt))

	if err != nil {
		return nil, err
	}

	err = json.Unmarshal(dBytes, &details)

	return details, err
}
