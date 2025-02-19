package steam_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"slices"
)

func GetAppNews(since int64, force bool) error {

	gana := nod.NewProgress("getting %s...", vangogh_integration.SteamAppNews)
	defer gana.Done()

	if force {
		since = -1
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.SteamAppIdProperty)
	if err != nil {
		return err
	}

	steamAppNewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppNews)
	if err != nil {
		return err
	}

	kvSteamAppNews, err := kevlar.New(steamAppNewsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	catalogAccountProducts, err := shared_data.GetCatalogAccountProducts(since)
	if err != nil {
		return err
	}

	steamAppIds := make([]string, 0, len(catalogAccountProducts))

	for gogId := range catalogAccountProducts {
		if sai, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok && sai != "" {
			steamAppIds = append(steamAppIds, sai)
		}
	}

	gana.TotalInt(len(steamAppIds))

	if err = fetch.Items(slices.Values(steamAppIds), reqs.SteamAppNews(), kvSteamAppNews, gana); err != nil {
		return err
	}

	return nil
}
