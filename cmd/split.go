package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/kvas"
	"strconv"
)

func split(sourcePt vangogh_products.ProductType, mt gog_media.Media, timestamp int64) error {
	vrPaged, err := vangogh_values.NewReader(sourcePt, mt)
	if err != nil {
		return err
	}

	modifiedIds := vrPaged.ModifiedAfter(timestamp, false)
	if len(modifiedIds) == 0 {
		fmt.Printf("skip split for not modified %s (%s)\n", sourcePt, mt)
		return nil
	}

	for _, id := range modifiedIds {

		splitPt := vangogh_products.SplitType(sourcePt)

		fmt.Printf("split %s (%s) %s into %s\n", sourcePt, mt, id, splitPt)

		productsGetter, err := vrPaged.ProductsGetter(id)

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
