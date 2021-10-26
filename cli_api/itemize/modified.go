package itemize

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
)

func Modified(
	since int64,
	pt vangogh_products.ProductType,
	mt gog_media.Media) (gost.StrSet, error) {

	ma := nod.Begin(" finding modified %s...", pt)
	defer ma.End()

	//licence products can only update through creation, and we've already handled
	//newly created in itemizeMissing func
	if pt == vangogh_products.LicenceProducts {
		return nil, nil
	}

	destUrl, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return nil, ma.EndWithError(err)
	}

	kv, err := kvas.NewJsonLocal(destUrl)
	if err != nil {
		return nil, ma.EndWithError(err)
	}

	modSet := gost.NewStrSetWith(kv.ModifiedAfter(since, false)...)

	ma.EndWithResult(itemizationResult(modSet))

	return modSet, nil
}
