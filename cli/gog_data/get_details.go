package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"iter"
	"maps"
	"net/http"
	"strconv"
)

func GetDetails(hc *http.Client, uat string, since int64) error {

	gda := nod.NewProgress("getting new or updated %s...", vangogh_integration.Details)
	defer gda.Done()

	detailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Details)
	if err != nil {
		return err
	}

	kvDetails, err := kevlar.New(detailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	newUpdatedDetails := make(map[string]any)

	newRequiredGameLicences, err := getNewRequiredGameLicences(kvDetails)
	if err != nil {
		return err
	}

	for id := range newRequiredGameLicences {
		newUpdatedDetails[id] = nil
	}

	newUpdatedAccountProducts, err := getNewUpdatedAccountPages(kvDetails)
	if err != nil {
		return err
	}

	for id := range newUpdatedAccountProducts {
		newUpdatedDetails[id] = nil
	}

	gda.TotalInt(len(newUpdatedDetails))

	if err = fetch.Items(maps.Keys(newUpdatedDetails), reqs.Details(hc, uat), kvDetails, gda); err != nil {
		return err
	}

	return reduceDetails(kvDetails, since)
}

func getNewRequiredGameLicences(kvDetails kevlar.KeyValues) (iter.Seq[string], error) {

	gnla := nod.NewProgress(" enumerating %s updates...", vangogh_integration.Licences)
	defer gnla.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return nil, err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.LicencesProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.RequiresGamesProperty)
	if err != nil {
		return nil, err
	}

	return func(yield func(string) bool) {
		for licenceId := range rdx.Keys(vangogh_integration.LicencesProperty) {

			if productType, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, licenceId); ok {
				switch productType {
				case "GAME":
					// do nothing
					continue
				case "PACK":
					// skip, account products will have the corresponding GAME products
					continue
				case "DLC":
					// replace DLC licence Id with GAME product that is required for this DLC
					if requiresGame, sure := rdx.GetLastVal(vangogh_integration.RequiresGamesProperty, licenceId); sure {
						if !kvDetails.Has(requiresGame) && !yield(requiresGame) {
							return
						}
					}
				}
			}

		}
	}, nil
}

func getNewUpdatedAccountPages(kvDetails kevlar.KeyValues) (iter.Seq[string], error) {

	gnuapa := nod.NewProgress(" enumerating %s updates...", vangogh_integration.AccountPage)
	defer gnuapa.Done()

	accountPagesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.AccountPage)
	if err != nil {
		return nil, err
	}

	kvAccountPages, err := kevlar.New(accountPagesDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	newUpdatedAccountProducts := make(map[string]any)

	gnuapa.TotalInt(kvAccountPages.Len())

	for page := range kvAccountPages.Keys() {
		pageNewUpdatedAccountProducts, err := getPageNewUpdatedAccountProducts(page, kvAccountPages, kvDetails)
		if err != nil {
			return nil, err
		}
		for id := range pageNewUpdatedAccountProducts {
			newUpdatedAccountProducts[id] = nil
		}
		gnuapa.Increment()
	}

	if len(newUpdatedAccountProducts) == 0 {
		gnuapa.EndWithResult("no updates found")
	} else {
		gnuapa.EndWithResult("found %d updates", len(newUpdatedAccountProducts))
	}

	return maps.Keys(newUpdatedAccountProducts), nil
}

func getPageNewUpdatedAccountProducts(page string, kvAccountPages, kvDetails kevlar.KeyValues) (iter.Seq[string], error) {
	rcAccountPage, err := kvAccountPages.Get(page)
	if err != nil {
		return nil, err
	}
	defer rcAccountPage.Close()

	var accountPage gog_integration.AccountPage
	if err = json.NewDecoder(rcAccountPage).Decode(&accountPage); err != nil {
		return nil, err
	}

	return func(yield func(string) bool) {
		for _, accountProduct := range accountPage.Products {
			id := strconv.Itoa(accountProduct.Id)
			if !kvDetails.Has(id) {
				if !yield(id) {
					return
				}
			}
			if accountProduct.IsNew || accountProduct.Updates > 0 {
				if !yield(id) {
					return
				}
			}
		}
	}, nil
}

func reduceDetails(kvDetails kevlar.KeyValues, since int64) error {
	rda := nod.Begin(" reducing %s...", vangogh_integration.Details)
	defer rda.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	detailProperties := []string{
		vangogh_integration.TitleProperty,
		vangogh_integration.FeaturesProperty,
		vangogh_integration.TagIdProperty,
		vangogh_integration.GOGReleaseDateProperty,
		vangogh_integration.ForumUrlProperty,
		vangogh_integration.ChangelogProperty,
	}

	rdx, err := redux.NewWriter(reduxDir, detailProperties...)
	if err != nil {
		return err
	}

	detailReductions := shared_data.InitReductions(detailProperties...)

	updatedDetails := kvDetails.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedDetails {
		if err = reduceDetailsProduct(id, kvDetails, detailReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, detailReductions)
}

func reduceDetailsProduct(id string, kvDetails kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcDetails, err := kvDetails.Get(id)
	if err != nil {
		return err
	}
	defer rcDetails.Close()

	var det gog_integration.Details
	if err = json.NewDecoder(rcDetails).Decode(&det); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.TitleProperty:
			values = []string{det.GetTitle()}
		case vangogh_integration.FeaturesProperty:
			values = det.GetFeatures()
		case vangogh_integration.TagIdProperty:
			values = det.GetTagIds()
		case vangogh_integration.GOGReleaseDateProperty:
			values = []string{det.GetGOGRelease()}
		case vangogh_integration.ForumUrlProperty:
			values = []string{det.GetForumUrl()}
		case vangogh_integration.ChangelogProperty:
			values = []string{det.GetChangelog()}
		}

		piv[property][id] = values
	}

	return nil
}
