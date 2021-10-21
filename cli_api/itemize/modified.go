package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/kvas"
)

func Modified(
	since int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) (gost.StrSet, error) {

	//licence products can only update through creation, and we've already handled
	//newly created in itemizeMissing func
	if pt == vangogh_products.LicenceProducts {
		return nil, nil
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return nil, err
	}

	kv, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return nil, err
	}

	modSet := gost.NewStrSetWith(kv.ModifiedAfter(since, false)...)

	return modSet, nil
}
