package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"time"
)

func GetOrderPages(force bool) error {
	gopa := nod.NewProgress("getting %s...", vangogh_integration.OrderPage)
	defer gopa.EndWithResult("done")

	hc, err := gogAuthHttpClient()
	if err != nil {
		return err
	}

	orderPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.OrderPage)
	if err != nil {
		return err
	}

	kvOrderPages, err := kevlar.New(orderPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = getGogPages(gog_integration.OrdersPageUrl, hc, kvOrderPages, gopa, force); err != nil {
		return err
	}

	return reduceOrderPages(kvOrderPages)
}

func reduceOrderPages(kvOrderPages kevlar.KeyValues) error {

	ropa := nod.NewProgress(" reducing %s...", vangogh_integration.OrderPage)
	defer ropa.EndWithResult("done")

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	orderProperties := []string{vangogh_integration.GOGOrderDateProperty}

	rdx, err := redux.NewWriter(reduxDir, orderProperties...)
	if err != nil {
		return err
	}

	ropa.TotalInt(kvOrderPages.Len())

	orderPagesReductions := initReductions(orderProperties...)

	for page := range kvOrderPages.Keys() {
		if err = reduceOrderPage(page, kvOrderPages, orderPagesReductions); err != nil {
			return err
		}

		ropa.Increment()
	}

	return writeReductions(rdx, orderPagesReductions)
}

func reduceOrderPage(page string, kvOrderPages kevlar.KeyValues, piv propertyIdValues) error {

	rcOrderPage, err := kvOrderPages.Get(page)
	if err != nil {
		return err
	}
	defer rcOrderPage.Close()

	var orderPage gog_integration.OrderPage
	if err = json.NewDecoder(rcOrderPage).Decode(&orderPage); err != nil {
		return err
	}

	for _, order := range orderPage.Orders {
		if order.Status != gog_integration.OrderStatusPurchase {
			continue
		}

		gogOrderDate := []string{time.Unix(order.Date, 0).Format(time.RFC3339)}

		for property := range piv {
			for _, orderProduct := range order.Products {
				switch property {
				case vangogh_integration.GOGOrderDateProperty:
					piv[property][orderProduct.Id] = gogOrderDate
				}
			}
		}
	}

	return nil
}
