package steam_data

import (
	"time"

	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
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

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.GetDataProperties()...)
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

	return nil
}
