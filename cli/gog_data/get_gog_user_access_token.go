package gog_data

import (
	"net/http"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogUserAccessToken(hc *http.Client) error {

	productType := vangogh_integration.GogUserAccessToken

	guata := nod.Begin("getting %s...", productType)
	defer guata.Done()

	gogUserAccessTokenDir, err := vangogh_integration.AbsProductTypeDir(productType)
	if err != nil {
		return err
	}

	kvGogUserAccessToken, err := kevlar.New(gogUserAccessTokenDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	uatId := productType.String()
	uatUrl := gog_integration.UserAccessTokenUrl()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	ptId := vangogh_integration.ProductTypeId(productType, uatId)

	if err = fetch.RequestSetValue(uatId, uatUrl, reqs.GogUserAccessToken(hc), kvGogUserAccessToken); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.VangoghGetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)
		if err = rdx.ReplaceValues(vangogh_integration.VangoghGetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		return nil
	}

	return nil
}
