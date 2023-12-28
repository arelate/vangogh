package reductions

import (
	"github.com/arelate/southern_light/gog_integration"
	"strconv"
	"time"

	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
)

func Orders(modifiedAfter int64) error {

	oa := nod.NewProgress(" %s...", vangogh_local_data.GOGOrderDateProperty)
	defer oa.End()

	rdx, err := vangogh_local_data.NewReduxWriter(vangogh_local_data.GOGOrderDateProperty)
	if err != nil {
		return oa.EndWithError(err)
	}

	vrOrders, err := vangogh_local_data.NewReader(vangogh_local_data.Orders)
	if err != nil {
		return oa.EndWithError(err)
	}

	gogOrderDates := make(map[string][]string, 0)

	var modifiedOrders []string
	if modifiedAfter > 0 {
		modifiedOrders = vrOrders.ModifiedAfter(modifiedAfter, false)
	} else {
		modifiedOrders = vrOrders.Keys()
	}

	oa.TotalInt(len(modifiedOrders))

	for _, orderId := range modifiedOrders {
		order, err := vrOrders.Order(orderId)
		if err != nil {
			return oa.EndWithError(err)
		}

		// only consider purchase orders, filter out gifts, refunds
		if order.Status != gog_integration.OrderStatusPurchase {
			continue
		}

		orderTimestamp, err := strconv.ParseInt(orderId, 10, 64)
		if err != nil {
			return oa.EndWithError(err)
		}

		orderDate := time.Unix(orderTimestamp, 0)

		for _, orderProduct := range order.Products {
			gogOrderDates[orderProduct.Id] = []string{orderDate.Format("2006.01.02 15:04:05")}
		}

		oa.Increment()
	}

	if err := rdx.BatchReplaceValues(vangogh_local_data.GOGOrderDateProperty, gogOrderDates); err != nil {
		return oa.EndWithError(err)
	}

	oa.EndWithResult("done")

	return nil
}
