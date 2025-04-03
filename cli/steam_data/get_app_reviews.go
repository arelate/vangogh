package steam_data

import (
	"encoding/json"
	"fmt"
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
	"strconv"
)

func GetAppReviews(steamGogIds map[string][]string, since int64, force bool) error {

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

	dataType := vangogh_integration.SteamAppReviews

	rara := nod.Begin(" reducing %s...", dataType)
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
		if !kvAppReviews.Has(steamAppId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, steamAppId))
			continue
		}

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
		case vangogh_integration.SteamReviewScoreProperty:
			values = []string{strconv.Itoa(sar.GetReviewScore())}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][gogId] = values
		}

	}

	return nil
}
