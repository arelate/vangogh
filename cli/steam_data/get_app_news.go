package steam_data

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"time"
)

func GetAppNews(steamGogIds map[string][]string, since int64) error {

	gana := nod.NewProgress("getting %s...", vangogh_integration.SteamAppNews)
	defer gana.Done()

	appNewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppNews)
	if err != nil {
		return err
	}

	kvAppNews, err := kevlar.New(appNewsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gana.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppNews(), kvAppNews, gana, true); err != nil {
		return err
	}

	return ReduceAppNews(kvAppNews, since)
}

func ReduceAppNews(kvAppNews kevlar.KeyValues, since int64) error {

	dataType := vangogh_integration.SteamAppNews

	rana := nod.Begin(" reducing %s...", dataType)
	defer rana.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.SteamAppNewsProperties()...)
	if err != nil {
		return err
	}

	appNewsReductions := shared_data.InitReductions(vangogh_integration.SteamAppNewsProperties()...)

	updatedAppNews := kvAppNews.Since(since, kevlar.Create, kevlar.Update)

	for steamAppId := range updatedAppNews {
		if !kvAppNews.Has(steamAppId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, steamAppId))
			continue
		}

		for gogId := range rdx.Match(map[string][]string{vangogh_integration.SteamAppIdProperty: {steamAppId}}, redux.FullMatch) {
			if err = reduceAppNewsProduct(gogId, steamAppId, kvAppNews, appNewsReductions); err != nil {
				return err
			}
		}
	}

	return shared_data.WriteReductions(rdx, appNewsReductions)
}

func reduceAppNewsProduct(gogId, steamAppId string, kvAppNews kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcAppNews, err := kvAppNews.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcAppNews.Close()

	var gnfar steam_integration.GetNewsForAppResponse
	if err = json.NewDecoder(rcAppNews).Decode(&gnfar); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.SteamLastCommunityUpdateProperty:
			for _, newsItem := range gnfar.AppNews.NewsItems {
				if newsItem.FeedType != compton_data.FeedTypeCommunityAnnouncement {
					continue
				}
				values = []string{time.Unix(newsItem.Date, 0).Format(time.RFC3339)}
				break
			}
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][gogId] = values
		}
	}

	return nil
}
