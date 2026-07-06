package gog_data

import (
	"encoding/json/v2"
	"errors"
	"fmt"
	"net/http"
	"strconv"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogAccountPages(hc *http.Client, uat string, since int64, force bool) error {
	gapa := nod.NewProgress("getting %s...", vangogh_integration.GogAccountPage)
	defer gapa.Done()

	gogAccountPagesDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.GogAccountPage)

	kvGogAccountPages, err := kevlar.New(gogAccountPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogPages(reqs.GogAccountPage(hc, uat), kvGogAccountPages, gapa, true); err != nil {
		return err
	}

	return ReduceGogAccountPages(kvGogAccountPages, since)
}

func ReduceGogAccountPages(kvGogAccountPages kevlar.KeyValues, since int64) error {

	pageType := vangogh_integration.GogAccountPage

	rapa := nod.Begin(" reducing %s...", pageType)
	defer rapa.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogAccountPageProperties()...)
	if err != nil {
		return err
	}

	accountPagesReductions := shared_data.InitReductions(vangogh_integration.GogAccountPageProperties()...)

	updatedAccountPages := kvGogAccountPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedAccountPages {

		if !kvGogAccountPages.Has(page) {
			nod.LogError(errors.New("missing: " + pageType.String() + ", " + page))
			continue
		}

		if err = reduceGogAccountPage(page, kvGogAccountPages, accountPagesReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, accountPagesReductions)
}

func reduceGogAccountPage(page string, kvGogAccountPages kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcGogAccountPage, err := kvGogAccountPages.Get(page)
	if err != nil {
		return err
	}
	defer rcGogAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.UnmarshalRead(rcGogAccountPage, &accountPage); err != nil {
		return err
	}

	// reduce tag names that are provided by account page, not product
	// and use any page to do that (any page would work equally well)
	piv[vangogh_integration.GogTagNameProperty] = make(map[string][]string)
	for _, tag := range accountPage.Tags {
		piv[vangogh_integration.GogTagNameProperty][tag.Id] = []string{tag.Name}
	}

	for ii, ap := range accountPage.Products {
		for property := range piv {

			var values []string

			switch property {
			case vangogh_integration.GogTagIdProperty:
				values = ap.GetTagIds()
			case vangogh_integration.GogTagNameProperty:
				// tag names are reduced at the page level, avoid resetting it here by skipping this property
				continue
			case vangogh_integration.GogSlugProperty:
				values = []string{ap.Slug}
			case vangogh_integration.GogAccountProductPageProperty:
				values = []string{page}
			case vangogh_integration.GogImageProperty:
				values = []string{gog_integration.ImageId(ap.GetImage())}
			case vangogh_integration.GogIsAccountProductProperty:
				values = []string{vangogh_integration.TrueValue}
			case vangogh_integration.GogAccountProductOrderProperty:
				order := accountProductOrder(ii, &accountPage)
				values = []string{fmt.Sprintf("%06d", order)}
			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][strconv.Itoa(ap.Id)] = values
			}
		}
	}

	return nil
}

func accountProductOrder(ii int, accountPage *gog_integration.AccountPage) int {
	return accountPage.ProductsPerPage*(accountPage.Page-1) + ii + 1
}
