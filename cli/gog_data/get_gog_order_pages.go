package gog_data

import (
	"encoding/json/v2"
	"errors"
	"net/http"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogOrderPages(hc *http.Client, uat string, since int64, force bool) error {
	gopa := nod.NewProgress("getting %s...", vangogh_integration.GogOrderPage)
	defer gopa.Done()

	gogOrderPagesDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogOrderPage)

	kvGogOrderPages, err := kevlar.New(gogOrderPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogPages(reqs.GogOrderPage(hc), kvGogOrderPages, gopa, force); err != nil {
		return err
	}

	return ReduceGogOrderPages(kvGogOrderPages, since)
}

func ReduceGogOrderPages(kvGogOrderPages kevlar.KeyValues, since int64) error {

	pageType := vangogh_integration.GogOrderPage

	ropa := nod.Begin(" reducing %s...", pageType)
	defer ropa.Done()

	properties := vangogh_integration.GogOrderPageProperties()
	properties = append(properties, vangogh_integration.GogIncludesGamesProperty)

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), properties...)
	if err != nil {
		return err
	}

	orderPagesReductions := shared_data.InitReductions(vangogh_integration.GogOrderPageProperties()...)

	updatedGogOrderPages := kvGogOrderPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedGogOrderPages {
		if !kvGogOrderPages.Has(page) {
			nod.LogError(errors.New("missing: " + pageType.String() + ", " + page))
			continue
		}

		if err = reduceGogOrderPage(page, kvGogOrderPages, orderPagesReductions, rdx); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, orderPagesReductions)
}

func reduceGogOrderPage(page string, kvGogOrderPages kevlar.KeyValues, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

	rcGogOrderPage, err := kvGogOrderPages.Get(page)
	if err != nil {
		return err
	}
	defer rcGogOrderPage.Close()

	var orderPage gog_integration.OrderPage
	if err = json.UnmarshalRead(rcGogOrderPage, &orderPage); err != nil {
		return err
	}

	for _, order := range orderPage.Orders {
		if order.Status != gog_integration.OrderStatusPurchase {
			continue
		}

		gogOrderDate := []string{time.Unix(order.Date, 0).Format(time.RFC3339)}

		for property := range piv {
			for _, orderProduct := range order.Products {

				id := orderProduct.Id

				switch property {
				case vangogh_integration.GogOrderDateProperty:
					piv[property][id] = gogOrderDate
					if includesGames, ok := rdx.GetAllValues(vangogh_integration.GogIncludesGamesProperty, id); ok {
						for _, igId := range includesGames {
							piv[property][igId] = gogOrderDate
						}
					}
				case vangogh_integration.GogOrderPageProductsProperty:
					piv[property][id] = []string{page}
				case vangogh_integration.GogImageProperty:
					// order image should be used only when not sourced from primary type (e.g. catalog)
					if !rdx.HasKey(vangogh_integration.GogImageProperty, id) {
						piv[property][id] = []string{gog_integration.ImageId(orderProduct.GetImage())}
					}
				}
			}
		}
	}

	return nil
}
