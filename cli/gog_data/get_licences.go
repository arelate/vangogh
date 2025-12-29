package gog_data

import (
	"encoding/json"
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

func GetLicences(hc *http.Client, userAccessToken string) error {

	productType := vangogh_integration.Licences

	gla := nod.Begin("getting %s...", productType)
	defer gla.Done()

	licencesDir, err := vangogh_integration.AbsProductTypeDir(productType)
	if err != nil {
		return err
	}

	kvLicences, err := kevlar.New(licencesDir, kevlar.JsonExt)
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

	ptId, err := vangogh_integration.ProductTypeId(productType, licencesId)
	if err != nil {
		return err
	}

	if err = fetch.RequestSetValue(licencesId, licencesUrl, reqs.Licenses(hc, userAccessToken), kvLicences); err != nil {

		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorMessageProperty, ptId, err.Error()); err != nil {
			return err
		}

		formattedNow := time.Now().UTC().Format(time.RFC3339)
		if err = rdx.ReplaceValues(vangogh_integration.GetDataErrorDateProperty, ptId, formattedNow); err != nil {
			return err
		}

		return nil
	}

	return ReduceLicences(kvLicences)
}

func ReduceLicences(kvLicences kevlar.KeyValues) error {

	rla := nod.Begin(" reducing %s...", vangogh_integration.Licences)
	defer rla.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.LicencesProperty)
	if err != nil {
		return err
	}

	key := vangogh_integration.LicencesProperty
	if err = rdx.MustHave(key); err != nil {
		return err
	}

	rcLicences, err := kvLicences.Get(vangogh_integration.Licences.String())
	if err != nil {
		return err
	}
	defer rcLicences.Close()

	var licences []string
	if err = json.NewDecoder(rcLicences).Decode(&licences); err != nil {
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
