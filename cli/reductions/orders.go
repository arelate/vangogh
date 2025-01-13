package reductions

import (
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"strconv"
	"time"

	"github.com/boggydigital/nod"
)

func Orders(modifiedAfter int64) error {

	oa := nod.NewProgress(" %s...", vangogh_integration.GOGOrderDateProperty)
	defer oa.End()

	rdx, err := vangogh_integration.NewReduxWriter(vangogh_integration.GOGOrderDateProperty)
	if err != nil {
		return oa.EndWithError(err)
	}

	vrOrders, err := vangogh_integration.NewProductReader(vangogh_integration.Orders)
	if err != nil {
		return oa.EndWithError(err)
	}

	gogOrderDates := make(map[string][]string, 0)

	var modifiedOrders []string
	if modifiedAfter > 0 {
		modifiedOrders, err = vrOrders.CreatedOrUpdatedAfter(modifiedAfter)
		if err != nil {
			return oa.EndWithError(err)
		}
	} else {
		modifiedOrders, err = vrOrders.Keys()
		if err != nil {
			return oa.EndWithError(err)
		}
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

	if err := rdx.BatchReplaceValues(vangogh_integration.GOGOrderDateProperty, gogOrderDates); err != nil {
		return oa.EndWithError(err)
	}

	oa.EndWithResult("done")

	return nil
}
