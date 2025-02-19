package steam_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
)

func GetAppReviews(steamGogIds map[string]string, since int64) error {

	gara := nod.NewProgress("getting %s...", vangogh_integration.SteamAppReviews)
	defer gara.Done()

	appReviewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppReviews)
	if err != nil {
		return err
	}

	kvSteamAppReviews, err := kevlar.New(appReviewsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gara.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppReviews(), kvSteamAppReviews, gara); err != nil {
		return err
	}

	return reduceAppReviews(kvSteamAppReviews, since)
}

func reduceAppReviews(kvAppReviews kevlar.KeyValues, since int64) error {

	rara := nod.Begin(" reducing %s...", vangogh_integration.SteamAppReviews)
	defer rara.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.SteamAppReviewsProperties()...)
	if err != nil {
		return err
	}

	steamAppReviewsReductions := shared_data.InitReductions(vangogh_integration.SteamAppReviewsProperties()...)

	updatedSteamAppReviews := kvAppReviews.Since(since, kevlar.Create, kevlar.Update)

	for steamAppId := range updatedSteamAppReviews {
		for gogId := range rdx.Match(map[string][]string{vangogh_integration.SteamAppIdProperty: {steamAppId}}, redux.FullMatch) {
			if err = reduceAppReviewsProduct(gogId, steamAppId, kvAppReviews, steamAppReviewsReductions); err != nil {
				return err
			}
		}
	}

	return shared_data.WriteReductions(rdx, steamAppReviewsReductions)
}

func reduceAppReviewsProduct(gogId, steamAppId string, kvAppReviews kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcSteamAppReview, err := kvAppReviews.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcSteamAppReview.Close()

	var sar steam_integration.AppReviews
	if err = json.NewDecoder(rcSteamAppReview).Decode(&sar); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.SteamReviewScoreDescProperty:
			values = []string{sar.GetReviewScoreDesc()}
		}

		piv[property][gogId] = values

	}

	return nil
}
