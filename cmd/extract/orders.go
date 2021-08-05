package extract

import (
	"fmt"
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"strconv"
	"time"
)

func Orders(modifiedAfter int64) error {
	fmt.Println("extracting order dates")
	exl, err := vangogh_extracts.NewList(vangogh_properties.GOGOrderDate)
	if err != nil {
		return err
	}

	vrOrders, err := vangogh_values.NewReader(vangogh_products.Orders, gog_media.Game)
	if err != nil {
		return err
	}

	gogOrderDates := make(map[string][]string, 0)

	var modifiedOrders []string
	if modifiedAfter > 0 {
		modifiedOrders = vrOrders.ModifiedAfter(modifiedAfter, false)
	} else {
		modifiedOrders = vrOrders.All()
	}

	for _, orderId := range modifiedOrders {
		order, err := vrOrders.Order(orderId)
		if err != nil {
			return err
		}

		orderTimestamp, err := strconv.Atoi(orderId)
		if err != nil {
			return err
		}

		orderDate := time.Unix(int64(orderTimestamp), 0)

		for _, orderProduct := range order.Products {
			gogOrderDates[orderProduct.Id] = []string{orderDate.Format("2006-01-02 15:04:05")}
		}
	}

	return exl.SetMany(vangogh_properties.GOGOrderDate, gogOrderDates)
}
