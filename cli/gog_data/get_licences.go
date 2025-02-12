package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"slices"
)

func GetLicences() error {
	gla := nod.Begin("getting %s...", vangogh_integration.Licences)
	defer gla.EndWithResult("done")

	licencesDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.Licences)
	if err != nil {
		return err
	}

	kvLicences, err := kevlar.New(licencesDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	if err = getGogAuthData(vangogh_integration.Licences.String(),
		gog_integration.LicencesUrl(),
		kvLicences); err != nil {
		return err
	}

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.LicencesProperty)
	if err != nil {
		return err
	}

	return reduceLicences(kvLicences, rdx)
}

func reduceLicences(kvLicences kevlar.KeyValues, rdx redux.Writeable) error {

	rla := nod.Begin(" reducing %s...", vangogh_integration.Licences)
	defer rla.EndWithResult("done")

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
