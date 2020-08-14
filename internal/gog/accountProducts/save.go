package accountProducts

import (
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Save(ap *AccountProduct, mt urls.MediaType) error {
	return storage.Save(ap, paths.AccountProduct(ap.ID, mt))
}
