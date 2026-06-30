package gog_data

import (
	"encoding/json/v2"
	"net/http"
	"slices"
	"time"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetGogLicences(hc *http.Client, userAccessToken string) error {

	productType := vangogh_integration.GogLicences

	gla := nod.Begin("getting %s...", productType)
	defer gla.Done()

	gogLicencesDir, err := vangogh_integration.AbsProductTypeDir(productType)
	if err != nil {
		return err
	}

	kvGogLicences, err := kevlar.New(gogLicencesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	licencesId := productType.String()
	licencesUrl := gog_integration.LicencesUrl()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GetDataProperties()...)
	if err != nil {
		return err
	}

	ptId := vangogh_integration.ProductTypeId(productType, licencesId)

	if err = fetch.RequestSetValue(licencesId, licencesUrl, reqs.GogLicenses(hc, userAccessToken), kvGogLicences); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.VangoghGetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)
		if err = rdx.ReplaceValues(vangogh_integration.VangoghGetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		return nil
	}

	return ReduceGogLicences(kvGogLicences)
}

func ReduceGogLicences(kvGogLicences kevlar.KeyValues) error {

	rla := nod.Begin(" reducing %s...", vangogh_integration.GogLicences)
	defer rla.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.GogLicencesProperty)
	if err != nil {
		return err
	}

	key := vangogh_integration.GogLicencesProperty
	if err = rdx.MustHave(key); err != nil {
		return err
	}

	rcGogLicences, err := kvGogLicences.Get(vangogh_integration.GogLicences.String())
	if err != nil {
		return err
	}
	defer rcGogLicences.Close()

	var licences []string
	if err = json.UnmarshalRead(rcGogLicences, &licences); err != nil {
		return err
	}

	licencesMap := make(map[string][]string, len(licences))
	for _, id := range licences {
		licencesMap[id] = []string{vangogh_integration.TrueValue}
	}

	if err = rdx.CutKeys(key, slices.Collect(rdx.Keys(key))...); err != nil {
		return err
	}

	return rdx.BatchAddValues(key, licencesMap)
}
