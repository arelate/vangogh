package accountProducts

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int, mediaType urls.MediaType) (ap *AccountProduct, err error) {
	apBytes, err := storage.Load(paths.AccountProduct(id, mediaType))

	if err != nil {
		return ap, err
	}

	err = json.Unmarshal(apBytes, &ap)

	return ap, err
}
