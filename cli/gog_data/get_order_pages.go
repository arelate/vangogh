package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"time"
)

func GetOrderPages(hc *http.Client, uat string, since int64, force bool) error {
	gopa := nod.NewProgress("getting %s...", vangogh_integration.OrderPage)
	defer gopa.Done()

	orderPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.OrderPage)
	if err != nil {
		return err
	}

	kvOrderPages, err := kevlar.New(orderPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogPages(reqs.OrderPage(hc, uat), kvOrderPages, gopa, force); err != nil {
		return err
	}

	return ReduceOrderPages(kvOrderPages, since)
}

func ReduceOrderPages(kvOrderPages kevlar.KeyValues, since int64) error {

	ropa := nod.Begin(" reducing %s...", vangogh_integration.OrderPage)
	defer ropa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GOGOrderPageProperties()...)
	if err != nil {
		return err
	}

	orderPagesReductions := shared_data.InitReductions(vangogh_integration.GOGOrderPageProperties()...)

	updatedOrderPages := kvOrderPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedOrderPages {
		if err = reduceOrderPage(page, kvOrderPages, orderPagesReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, orderPagesReductions)
}

func reduceOrderPage(page string, kvOrderPages kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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
