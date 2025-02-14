package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"time"
)

func GetOrderPages(hc *http.Client, userAccessToken string, since int64, force bool) error {
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

	if err = fetchGogPages(gog_integration.OrdersPageUrl, hc, http.MethodGet, userAccessToken, kvOrderPages, gopa, force); err != nil {
		return err
	}

	return reduceOrderPages(kvOrderPages, since)
}

func reduceOrderPages(kvOrderPages kevlar.KeyValues, since int64) error {

	ropa := nod.Begin(" reducing %s...", vangogh_integration.OrderPage)
	defer ropa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	orderProperties := []string{vangogh_integration.GOGOrderDateProperty}

	rdx, err := redux.NewWriter(reduxDir, orderProperties...)
	if err != nil {
		return err
	}

	orderPagesReductions := initReductions(orderProperties...)

	updatedOrderPages := kvOrderPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedOrderPages {
		if err = reduceOrderPage(page, kvOrderPages, orderPagesReductions); err != nil {
			return err
		}
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
