package cli

import (
	"bytes"
	"encoding/json"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"sort"
	"strconv"
)

func split(sourcePt vangogh_local_data.ProductType, timestamp int64) error {

	splitPt := vangogh_local_data.SplitProductType(sourcePt)
	if splitPt == vangogh_local_data.UnknownProductType {
		return nil
	}

	spa := nod.NewProgress(" splitting %s...", sourcePt)
	defer spa.End()

	vrPaged, err := vangogh_local_data.NewProductReader(sourcePt)
	if err != nil {
		return err
	}

	modifiedIds := vrPaged.ModifiedAfter(timestamp, false)
	if len(modifiedIds) == 0 {
		spa.EndWithResult("unchanged")
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

	spa.TotalInt(len(modifiedIds))

	for _, id := range modifiedIds {

		productsGetter, err := vrPaged.ProductsGetter(id)

		if err != nil {
			return spa.EndWithError(err)
		}

		detailDstUrl, err := vangogh_local_data.AbsLocalProductTypeDir(splitPt)
		if err != nil {
			return spa.EndWithError(err)
		}

		kvDetail, err := kvas.NewKeyValues(detailDstUrl, kvas.JsonExt)
		if err != nil {
			return spa.EndWithError(err)
		}

		products := productsGetter.GetProducts()

		if sourcePt == vangogh_local_data.Licences {
			spa.TotalInt(len(products))
		}

		for _, product := range products {
			buf := new(bytes.Buffer)
			if err := json.NewEncoder(buf).Encode(product); err != nil {
				return spa.EndWithError(err)
			}
			if err := kvDetail.Set(strconv.Itoa(product.GetId()), buf); err != nil {
				return spa.EndWithError(err)
			}
			if sourcePt == vangogh_local_data.Licences {
				spa.Increment()
			}
		}

		if sourcePt != vangogh_local_data.Licences {
			spa.Increment()
		}
	}

	spa.EndWithResult("done")

	return nil
}
