package reductions

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"strconv"
	"strings"
)

func SteamAppId(since int64) error {

	saia := nod.NewProgress(" %s...", vangogh_local_data.SteamAppIdProperty)
	defer saia.End()

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.SteamAppIdProperty)
	if err != nil {
		return saia.EndWithError(err)
	}

	vrSteamAppList, err := vangogh_local_data.NewProductReader(vangogh_local_data.SteamAppList)
	if err != nil {
		return saia.EndWithError(err)
	}

	vrCatalogProducts, err := vangogh_local_data.NewProductReader(vangogh_local_data.CatalogProducts)
	if err != nil {
		return saia.EndWithError(err)
	}

	sal, err := vrSteamAppList.SteamAppList()
	if err != nil {
		return saia.EndWithError(err)
	}

	appMap := GetAppListResponseToMap(sal)
	gogSteamAppId := make(map[string][]string)

	updated, err := vrCatalogProducts.CreatedOrUpdatedAfter(since)
	if err != nil {
		return saia.EndWithError(err)
	}

	saia.TotalInt(len(updated))

	for _, id := range updated {
		title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)
		if !ok {
			saia.Increment()
			continue
		}

		title = normalizeTitle(title)

		if appId, ok := appMap[title]; ok {
			gogSteamAppId[id] = []string{strconv.Itoa(int(appId))}
		}

		saia.Increment()
	}

	if err := rdx.BatchReplaceValues(vangogh_local_data.SteamAppIdProperty, gogSteamAppId); err != nil {
		return saia.EndWithError(err)
	}

	saia.EndWithResult("done")

	return nil
}

var filterCharacters = []string{"®", "™", "!", "?", "'", "’"}

func GetAppListResponseToMap(galr *steam_integration.GetAppListResponse) map[string]uint32 {

	appsMap := make(map[string]uint32, len(galr.AppList.Apps))

	for _, app := range galr.AppList.Apps {

		an := app.Name

		if an == "" {
			continue
		}

		an = normalizeTitle(an)

		appsMap[an] = app.AppId
	}

	return appsMap
}

func normalizeTitle(s string) string {
	// normalize the app name by removing superfluous characters
	for _, fc := range filterCharacters {
		s = strings.Replace(s, fc, "", -1)
	}
	// lower the case to normalize case differences
	s = strings.ToLower(s)
	return s
}
