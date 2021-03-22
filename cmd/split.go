package cmd

import (
	"bytes"
	"encoding/json"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/kvas"
	"log"
	"strconv"
)

func split(pagedPt vangogh_products.ProductType, mt gog_media.Media, timestamp int64) error {
	vrPaged, err := vangogh_values.NewReader(pagedPt, mt)
	if err != nil {
		return err
	}

	modifiedPageIds := vrPaged.ModifiedAfter(timestamp)
	if len(modifiedPageIds) == 0 {
		log.Printf("skip split for not modified %s (%s) pages", pagedPt, mt)
		return nil
	}

	for _, page := range modifiedPageIds {

		splitPt := vangogh_products.SplitType(pagedPt)

		log.Printf("split %s (%s) %s into %s\n", pagedPt, mt, page, splitPt)

		productsGetter, err := vrPaged.ProductGetter(page)

		if err != nil {
			return err
		}

		detailDstUrl, err := vangogh_urls.LocalProductsDir(splitPt, mt)
		if err != nil {
			return nil
		}

		kvDetail, err := kvas.NewJsonLocal(detailDstUrl)
		if err != nil {
			return err
		}

		for _, product := range productsGetter.GetProducts() {
			buf := new(bytes.Buffer)
			if err := json.NewEncoder(buf).Encode(product); err != nil {
				return err
			}
			if err := kvDetail.Set(strconv.Itoa(product.GetId()), buf); err != nil {
				return err
			}
		}
	}

	return nil
}
