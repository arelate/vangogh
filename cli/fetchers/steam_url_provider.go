package fetchers

import (
	"github.com/arelate/steam_integration"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"net/url"
	"strconv"
)

func GOGIdToSteamApp(gogId string, rxa kvas.ReduxAssets) uint32 {
	if appIdStr, ok := rxa.GetFirstVal(vangogh_local_data.SteamAppIdProperty, gogId); ok {
		if appId, err := strconv.ParseUint(appIdStr, 10, 32); err == nil {
			return uint32(appId)
		}
	}
	return 0
}

type SteamUrlProvider struct {
	rxa     kvas.ReduxAssets
	urlFunc steam_integration.SteamUrlFunc
}

func (sup *SteamUrlProvider) DefaultSourceUrl(gogId string) *url.URL {

	if appId := GOGIdToSteamApp(gogId, sup.rxa); appId > 0 {
		return sup.urlFunc(appId)
	}

	return nil
}
