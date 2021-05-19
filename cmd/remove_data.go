package cmd

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/kvas"
	"log"
)

func removeData(ids map[string]bool, pt vangogh_products.ProductType, mt gog_media.Media) error {
	ptDir, err := vangogh_urls.LocalProductsDir(pt, mt)
	if err != nil {
		return err
	}
	kvPt, err := kvas.NewJsonLocal(ptDir)
	if err != nil {
		return err
	}

	for id, ok := range ids {
		if !ok {
			continue
		}
		log.Printf("remove %s (%s) id %s", pt, mt, id)
		if err := kvPt.Remove(id); err != nil {
			return err
		}
	}

	return nil
}
