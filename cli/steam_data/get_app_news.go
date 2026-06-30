package steam_data

import (
	"encoding/json/v2"
	"errors"
	"iter"
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

	return ReduceAppNews(maps.Keys(steamGogIds), kvAppNews)
}

func ReduceAppNews(steamAppIds iter.Seq[string], kvAppNews kevlar.KeyValues) error {

	dataType := vangogh_integration.SteamAppNews

	rana := nod.NewProgress(" reducing %s...", dataType)
	defer rana.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.SteamAppNewsProperties()...)
	if err != nil {
		return err
	}

	appNewsReductions := shared_data.InitReductions(vangogh_integration.SteamAppNewsProperties()...)

	for steamAppId := range steamAppIds {
		if !kvAppNews.Has(steamAppId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + steamAppId))
			rana.Increment()
			continue
		}

		if err = reduceAppNewsProduct(steamAppId, kvAppNews, appNewsReductions); err != nil {
			return err
		}

		rana.Increment()
	}

	return shared_data.WriteReductions(rdx, appNewsReductions)
}

func reduceAppNewsProduct(steamAppId string, kvAppNews kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcAppNews, err := kvAppNews.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcAppNews.Close()

	var gnfar steam_integration.GetNewsForAppResponse
	if err = json.UnmarshalRead(rcAppNews, &gnfar); err != nil {
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
			piv[property][steamAppId] = values
		}
	}

	return nil
}
