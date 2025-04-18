package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"slices"
)

func GetLicences(hc *http.Client, userAccessToken string) error {

	gla := nod.Begin("getting %s...", vangogh_integration.Licences)
	defer gla.Done()

	licencesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Licences)
	if err != nil {
		return err
	}

	kvLicences, err := kevlar.New(licencesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	licencesId := vangogh_integration.Licences.String()
	licencesUrl := gog_integration.LicencesUrl()

	if err = fetch.RequestSetValue(licencesId, licencesUrl, reqs.Licenses(hc, userAccessToken), kvLicences); err != nil {
		return err
	}

	return ReduceLicences(kvLicences)
}

func ReduceLicences(kvLicences kevlar.KeyValues) error {

	rla := nod.Begin(" reducing %s...", vangogh_integration.Licences)
	defer rla.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.LicencesProperty)
	if err != nil {
		return err
	}

	key := vangogh_integration.LicencesProperty
	if err := rdx.MustHave(key); err != nil {
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
