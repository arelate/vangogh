package steam_data

import (
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
)

func GetAppDetails(since int64, force bool) error {

	gada := nod.NewProgress("getting %s...", vangogh_integration.SteamAppDetails)
	defer gada.Done()

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

	steamAppDetailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppDetails)
	if err != nil {
		return err
	}

	kvSteamAppDetails, err := kevlar.New(steamAppDetailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	var newSteamAppIds []string

	for gogId := range rdx.Keys(vangogh_integration.SteamAppIdProperty) {
		if steamAppIds, ok := rdx.GetAllValues(vangogh_integration.SteamAppIdProperty, gogId); ok {
			for _, steamAppId := range steamAppIds {
				if kvSteamAppDetails.Has(steamAppId) && !force {
					continue
				}
				newSteamAppIds = append(newSteamAppIds, steamAppId)
			}
		}
	}

	// TODO: Save errors and dates and don't request them again for 30 days
	if itemErrs := fetch.Items(steam_integration.AppDetailsUrl, http.DefaultClient, http.MethodGet, "", kvSteamAppDetails, gada, newSteamAppIds...); len(itemErrs) > 0 {
		return fmt.Errorf("get %s errors: %v", vangogh_integration.ApiProductsV2, itemErrs)
	}

	return nil
}
