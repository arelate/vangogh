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
	"strconv"
)

func GetAccountPages(force bool) error {
	gapa := nod.NewProgress("getting %s...", vangogh_integration.AccountPage)
	defer gapa.Done()

	hc, err := gogAuthHttpClient()
	if err != nil {
		return err
	}

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return err
	}

	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = fetchGogPages(gog_integration.AccountPageUrl, hc, http.MethodGet, kvAccountPages, gapa, force); err != nil {
		return err
	}

	return reduceAccountPages(kvAccountPages)
}

func reduceAccountPages(kvAccountPages kevlar.KeyValues) error {

	rapa := nod.NewProgress(" reducing %s...", vangogh_integration.AccountPage)
	defer rapa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	accountProductProperties := []string{
		vangogh_integration.TagIdProperty,
		vangogh_integration.TagNameProperty,
	}

	rdx, err := redux.NewWriter(reduxDir, accountProductProperties...)
	if err != nil {
		return err
	}

	rapa.TotalInt(kvAccountPages.Len())

	accountPagesReductions := initReductions(accountProductProperties...)

	for page := range kvAccountPages.Keys() {
		if err = reduceAccountPage(page, kvAccountPages, accountPagesReductions); err != nil {
			return err
		}

		rapa.Increment()
	}

	return writeReductions(rdx, accountPagesReductions)
}

func reduceAccountPage(page string, kvAccountPages kevlar.KeyValues, piv propertyIdValues) error {

	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return err
	}
	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.NewDecoder(rcAccountPage).Decode(&accountPage); err != nil {
		return err
	}

	accountPageReduction := make(map[string]map[string][]string)

	// reduce tag names that are provided by account page, not product
	// and use first page to do that (any page would work equally well)
	if page == "1" {
		accountPageReduction[vangogh_integration.TagNameProperty] = make(map[string][]string)
		for _, tag := range accountPage.Tags {
			accountPageReduction[vangogh_integration.TagNameProperty][tag.Id] = []string{tag.Name}
		}
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
			}

			piv[property][strconv.Itoa(ap.Id)] = values
		}
	}

	return nil

}
