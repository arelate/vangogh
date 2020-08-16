package products

import (
	"encoding/json"
	"github.com/boggydigital/vangogh/internal/gog/index"
	"github.com/boggydigital/vangogh/internal/gog/media"
	"github.com/boggydigital/vangogh/internal/gog/paths"
	"github.com/boggydigital/vangogh/internal/storage"
)

func Load(id int, mt media.Type) (p *Product, err error) {
	pBytes, err := storage.Load(paths.Product(id, mt))

	if err != nil {
		return nil, err
	}

	err = json.Unmarshal(pBytes, &p)

	return p, err
}

func LoadIndex() error {
	return index.Load(paths.ProductIndex(), &indexes)
}
