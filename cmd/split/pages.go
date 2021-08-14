package split

import (
	"bytes"
	"encoding/json"
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/kvas"
	"sort"
	"strconv"
)

func Pages(sourcePt vangogh_products.ProductType, mt gog_media.Media, timestamp int64) error {
	vrPaged, err := vangogh_values.NewReader(sourcePt, mt)
	if err != nil {
		return err
	}

	modifiedIds := vrPaged.ModifiedAfter(timestamp, false)
	if len(modifiedIds) == 0 {
		fmt.Printf("no need to split unchanged %s (%s)\n", sourcePt, mt)
		return nil
	}

	// split operates on pages and ids are expected to be numerical...
	intIds := make([]int, 0, len(modifiedIds))
	for _, id := range modifiedIds {
		inv, err := strconv.Atoi(id)
		if err == nil {
			intIds = append(intIds, inv)
		}
	}

	// ...however if some were not - just use the original modifiedIds set
	if len(intIds) == len(modifiedIds) {
		sort.Ints(intIds)
		modifiedIds = make([]string, 0, len(intIds))
		for _, id := range intIds {
			modifiedIds = append(modifiedIds, strconv.Itoa(id))
		}
	}

	for _, id := range modifiedIds {

		splitPt := vangogh_products.SplitType(sourcePt)

		fmt.Printf("\rsplit %s (%s) %s... ", sourcePt, mt, id)

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

	fmt.Println("done")

	return nil
}
