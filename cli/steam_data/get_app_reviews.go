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

func GetAppReviews(steamGogIds map[string]string, since int64, force bool) error {

	gara := nod.NewProgress("getting %s...", vangogh_integration.SteamAppReviews)
	defer gara.Done()

	appReviewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppReviews)
	if err != nil {
		return err
	}

	kvAppReviews, err := kevlar.New(appReviewsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gara.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppReviews(), kvAppReviews, gara, force); err != nil {
		return err
	}

	return ReduceAppReviews(kvAppReviews, since)
}

func ReduceAppReviews(kvAppReviews kevlar.KeyValues, since int64) error {

	rara := nod.Begin(" reducing %s...", vangogh_integration.SteamAppReviews)
	defer rara.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	// need to append SteamAppId property to allow mapping to GOG ids
	properties := append(vangogh_integration.SteamAppReviewsProperties(),
		vangogh_integration.SteamAppIdProperty)

	rdx, err := redux.NewWriter(reduxDir, properties...)
	if err != nil {
		return err
	}

	appReviewsReductions := shared_data.InitReductions(vangogh_integration.SteamAppReviewsProperties()...)

	updatedAppReviews := kvAppReviews.Since(since, kevlar.Create, kevlar.Update)

	for steamAppId := range updatedAppReviews {
		for gogId := range rdx.Match(map[string][]string{vangogh_integration.SteamAppIdProperty: {steamAppId}}, redux.FullMatch) {
			if err = reduceAppReviewsProduct(gogId, steamAppId, kvAppReviews, appReviewsReductions); err != nil {
				return err
			}
		}
	}

	return shared_data.WriteReductions(rdx, appReviewsReductions)
}

func reduceAppReviewsProduct(gogId, steamAppId string, kvAppReviews kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcAppReview, err := kvAppReviews.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcAppReview.Close()

	var sar steam_integration.AppReviews
	if err = json.NewDecoder(rcAppReview).Decode(&sar); err != nil {
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
