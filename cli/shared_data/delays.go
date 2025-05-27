package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"time"
)

const (
	defaultErrorDelayDays  = 7
	defaultUpdateDelayDays = 30
)

var errorsDelayDays = map[vangogh_integration.ProductType]int{
	vangogh_integration.CatalogPage: 0, // this type errors are exceptional and most likely indicate transport or origin issue
	vangogh_integration.OrderPage:   0, // same as above
	vangogh_integration.AccountPage: 0, // same as above
	vangogh_integration.Details:     0, // same as above
}

var updatesDelayDays = map[vangogh_integration.ProductType]int{
	vangogh_integration.CatalogPage:  0, // this is foundational data and should always be updated
	vangogh_integration.OrderPage:    0, // same as above
	vangogh_integration.AccountPage:  0, // same as above
	vangogh_integration.Details:      0, // same as above
	vangogh_integration.SteamAppNews: 1,
}

func ShouldUpdate(id string, pt vangogh_integration.ProductType, rdx redux.Readable) (bool, error) {

	if err := rdx.MustHave(vangogh_integration.GetDataLastUpdatedProperty); err != nil {
		return false, err
	}

	ptId, err := vangogh_integration.ProductTypeId(pt, id)
	if err != nil {
		return false, err
	}

	if !rdx.HasKey(vangogh_integration.GetDataLastUpdatedProperty, ptId) {
		return true, nil
	}

	var updateDelayDays int
	if udd, sure := updatesDelayDays[pt]; sure {
		updateDelayDays = udd
	} else {
		updateDelayDays = defaultUpdateDelayDays
	}

	gdlut, err := rdx.ParseLastValTime(vangogh_integration.GetDataLastUpdatedProperty, ptId)
	if err != nil {
		return false, err
	}

	if time.Since(gdlut).Hours()/24 > float64(updateDelayDays) {
		nod.Log("updating %s %s, last update: %s", pt, id, gdlut.Format(time.RFC3339))
		return true, nil
	} else {
		return false, nil
	}

}

func ShouldSkip(id string, pt vangogh_integration.ProductType, rdx redux.Writeable) (bool, error) {

	if err := rdx.MustHave(vangogh_integration.GetDataErrorDateProperty,
		vangogh_integration.GetDataErrorMessageProperty); err != nil {
		return false, err
	}

	ptId, err := vangogh_integration.ProductTypeId(pt, id)
	if err != nil {
		return false, err
	}

	if !rdx.HasKey(vangogh_integration.GetDataErrorDateProperty, ptId) {
		return false, nil
	}

	var errorDelayDays int
	if edd, sure := errorsDelayDays[pt]; sure {
		errorDelayDays = edd
	} else {
		errorDelayDays = defaultErrorDelayDays
	}

	gdet, err := rdx.ParseLastValTime(vangogh_integration.GetDataErrorDateProperty, ptId)
	if err != nil {
		return false, err
	}

	if time.Since(gdet).Hours()/24 < float64(errorDelayDays) {

		nod.Log("skipping current %s %s error last encountered: %s", pt, id, gdet.Format(time.RFC3339))
		return true, nil
	} else {

		nod.Log("clearing %s %s error last encountered: %s", pt, id, gdet.Format(time.RFC3339))

		if err = rdx.CutKeys(vangogh_integration.GetDataErrorDateProperty, ptId); err != nil {
			return false, err
		}
		if err = rdx.CutKeys(vangogh_integration.GetDataErrorMessageProperty, ptId); err != nil {
			return false, err
		}

		return false, nil
	}
}
