package steam_data

import (
	"encoding/json"
	"errors"
	"maps"
	"time"

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
)

func GetAppNews(steamGogIds map[string][]string, force bool) error {

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

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppNews(), kvAppNews, gana, force); err != nil {
		return err
	}

	return ReduceAppNews(steamGogIds, kvAppNews)
}

func ReduceAppNews(steamGogIds map[string][]string, kvAppNews kevlar.KeyValues) error {

	dataType := vangogh_integration.SteamAppNews

	rana := nod.NewProgress(" reducing %s...", dataType)
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

	rana.TotalInt(len(steamGogIds))

	for steamAppId, gogIds := range steamGogIds {
		if !kvAppNews.Has(steamAppId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + steamAppId))
			rana.Increment()
			continue
		}

		if err = reduceAppNewsProduct(gogIds, steamAppId, kvAppNews, appNewsReductions); err != nil {
			return err
		}

		rana.Increment()
	}

	return shared_data.WriteReductions(rdx, appNewsReductions)
}

func reduceAppNewsProduct(gogIds []string, steamAppId string, kvAppNews kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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

		for _, gogId := range gogIds {
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
	}

	return nil
}
