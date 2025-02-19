package steam_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"maps"
)

func GetAppReviews(steamGogIds map[string]string) error {

	gara := nod.NewProgress("getting %s...", vangogh_integration.SteamAppReviews)
	defer gara.Done()

	steamAppReviewsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppReviews)
	if err != nil {
		return err
	}

	kvSteamAppReviews, err := kevlar.New(steamAppReviewsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gara.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppReviews(), kvSteamAppReviews, gara); err != nil {
		return err
	}

	return nil
}
