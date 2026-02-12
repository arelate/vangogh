package gog_data

import (
	"encoding/json/v2"
	"errors"
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

func GetAccountPages(hc *http.Client, uat string, since int64, force bool) error {
	gapa := nod.NewProgress("getting %s...", vangogh_integration.AccountPage)
	defer gapa.Done()

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return err
	}

	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogPages(reqs.AccountPage(hc, uat), kvAccountPages, gapa, true); err != nil {
		return err
	}

	return ReduceAccountPages(kvAccountPages, since)
}

func ReduceAccountPages(kvAccountPages kevlar.KeyValues, since int64) error {

	pageType := vangogh_integration.AccountPage

	rapa := nod.Begin(" reducing %s...", pageType)
	defer rapa.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GOGAccountPageProperties()...)
	if err != nil {
		return err
	}

	accountPagesReductions := shared_data.InitReductions(vangogh_integration.GOGAccountPageProperties()...)

	updatedAccountPages := kvAccountPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedAccountPages {

		if !kvAccountPages.Has(page) {
			nod.LogError(errors.New("missing: " + pageType.String() + ", " + page))
			continue
		}

		if err = reduceAccountPage(page, kvAccountPages, accountPagesReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, accountPagesReductions)
}

func reduceAccountPage(page string, kvAccountPages kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return err
	}
	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.UnmarshalRead(rcAccountPage, &accountPage); err != nil {
		return err
	}

	// reduce tag names that are provided by account page, not product
	// and use any page to do that (any page would work equally well)
	piv[vangogh_integration.TagNameProperty] = make(map[string][]string)
	for _, tag := range accountPage.Tags {
		piv[vangogh_integration.TagNameProperty][tag.Id] = []string{tag.Name}
	}

	for _, ap := range accountPage.Products {
		for property := range piv {

			var values []string

			switch property {
			case vangogh_integration.TagIdProperty:
				values = ap.GetTagIds()
			case vangogh_integration.TagNameProperty:
				// tag names are reduced at the page level, avoid resetting it here by skipping this property
				continue
			case vangogh_integration.SlugProperty:
				values = []string{ap.Slug}
			case vangogh_integration.AccountPageProductsProperty:
				values = []string{page}
			case vangogh_integration.ImageProperty:
				values = []string{gog_integration.ImageId(ap.GetImage())}

			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][strconv.Itoa(ap.Id)] = values
			}
		}
	}

	return nil
}
