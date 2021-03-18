package cmd

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/kvas"
	"log"
	"strconv"
)

func split(mainPt vangogh_types.ProductType, mt gog_types.Media) error {
	vrMain, err := vangogh_values.NewReader(mainPt, mt)
	if err != nil {
		return err
	}

	for _, page := range vrMain.All() {

		splitPt := vangogh_types.SplitProductType(mainPt)

		log.Printf("split %s (%s) %s into %s\n", mainPt, mt, page, splitPt)

		var productsGetter gog_types.ProductsGetter

		switch mainPt {
		case vangogh_types.StorePage:
			productsGetter, err = vrMain.StorePage(page)
		case vangogh_types.AccountPage:
			productsGetter, err = vrMain.AccountStorePage(page)
		case vangogh_types.WishlistPage:
			productsGetter, err = vrMain.WishlistPage(page)
		default:
			return fmt.Errorf("splitting page is not supported for type %s", mainPt)
		}

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
