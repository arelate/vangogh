package products

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int, mediaType urls.MediaType) (p *Product, err error) {
	pBytes, err := storage.Load(paths.Product(id, mediaType))

	if err != nil {
		return p, err
	}

	err = json.Unmarshal(pBytes, &p)

	return p, err
}
