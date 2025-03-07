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
	"strconv"
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

	if err = fetchGogPages(reqs.AccountPage(hc, uat), kvAccountPages, gapa, force); err != nil {
		return err
	}

	return ReduceAccountPages(kvAccountPages, since)
}

func ReduceAccountPages(kvAccountPages kevlar.KeyValues, since int64) error {

	rapa := nod.Begin(" reducing %s...", vangogh_integration.AccountPage)
	defer rapa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GOGAccountPageProperties()...)
	if err != nil {
		return err
	}

	accountPagesReductions := shared_data.InitReductions(vangogh_integration.GOGAccountPageProperties()...)

	updatedAccountPages := kvAccountPages.Since(since, kevlar.Create, kevlar.Update)

	for page := range updatedAccountPages {
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
	if err = json.NewDecoder(rcAccountPage).Decode(&accountPage); err != nil {
		return err
	}

	//accountPageReduction := make(map[string]map[string][]string)

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
			}

			piv[property][strconv.Itoa(ap.Id)] = values
		}
	}

	return nil

}
