package steam_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"maps"
)

func GetAppNews(steamGogIds map[string][]string) error {

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

	return nil
}
