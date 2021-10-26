package remove

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
)

func Data(ids []string, pt vangogh_products.ProductType, mt gog_media.Media) error {
	ptDir, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}
	kvPt, err := kvas.NewJsonLocal(ptDir)
	if err != nil {
		return err
	}

	for _, id := range ids {
		//log.Printf("remove %s (%s) id %s", pt, mt, id)
		if err := kvPt.Remove(id); err != nil {
			return err
		}
	}

	return nil
}
