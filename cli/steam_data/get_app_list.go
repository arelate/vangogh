package steam_data

import (
	"encoding/json"
	"strconv"
	"time"

	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetAppList() error {

	productType := vangogh_integration.SteamAppList

	gala := nod.Begin("getting %s...", productType)
	defer gala.Done()

	appListDir, err := vangogh_integration.AbsProductTypeDir(productType)
	if err != nil {
		return err
	}

	kvAppList, err := kevlar.New(appListDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	appListId := productType.String()
	appListUrl := steam_integration.AppListUrl()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	ptId, err := vangogh_integration.ProductTypeId(productType, appListId)
	if err != nil {
		return err
	}

	if err = fetch.RequestSetValue(appListId, appListUrl, reqs.SteamAppList(), kvAppList); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)
		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		return nil
	}

	return ReduceAppList(kvAppList)
}

func ReduceAppList(kvAppList kevlar.KeyValues) error {

	productType := vangogh_integration.SteamAppList

	rala := nod.NewProgress(" reducing %s...", productType)
	defer rala.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.SteamAppIdProperty,
		vangogh_integration.TitleProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.IsDemoProperty)
	if err != nil {
		return err
	}

	rcLicences, err := kvAppList.Get(vangogh_integration.SteamAppList.String())
	if err != nil {
		return err
	}
	defer rcLicences.Close()

	var appList steam_integration.GetAppListV2Response
	if err = json.NewDecoder(rcLicences).Decode(&appList); err != nil {
		return err
	}

	steamAppIds := make(map[string][]string)

	rala.TotalInt(rdx.Len(vangogh_integration.TitleProperty))

	for gogId := range rdx.Keys(vangogh_integration.TitleProperty) {

		if demo, ok := rdx.GetLastVal(vangogh_integration.IsDemoProperty, gogId); ok && demo == vangogh_integration.TrueValue {
			rala.Increment()
			continue
		}

		if sids, ok := rdx.GetAllValues(vangogh_integration.SteamAppIdProperty, gogId); ok && len(sids) > 0 {
			rala.Increment()
			continue
		}

		var gogTitle string
		if tp, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, gogId); ok && tp != "" {
			gogTitle = tp
		} else {
			rala.Increment()
			continue
		}

		for _, app := range appList.AppList.Apps {
			if app.Name == gogTitle {
				steamAppIds[gogId] = []string{strconv.FormatInt(int64(app.AppId), 10)}
				break
			}
		}

		rala.Increment()
	}

	if len(steamAppIds) > 0 {
		if err = rdx.BatchReplaceValues(vangogh_integration.SteamAppIdProperty, steamAppIds); err != nil {
			return err
		}
	}

	return nil
}
