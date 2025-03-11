package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
)

func ReduceTypes() error {

	ta := nod.Begin(" reducing %s...", vangogh_integration.TypesProperty)
	defer ta.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir,
		vangogh_integration.SteamAppIdProperty,
		vangogh_integration.TypesProperty)
	if err != nil {
		return err
	}

	steamAppIdGogIds := getSteamAppIdGogIds(rdx)

	idsTypes := make(map[string][]string)

	for pt := range vangogh_integration.AllProductTypes() {

		if pt == vangogh_integration.UnknownProductType {
			continue
		}

		ptDir, err := vangogh_integration.AbsProductTypeDir(pt)
		if err != nil {
			return err
		}

		kvPt, err := kevlar.New(ptDir, kevlar.JsonExt)
		if err != nil {
			return err
		}

		for id := range kvPt.Keys() {

			if idsTypes[id] == nil {
				idsTypes[id] = make([]string, 0)
			}

			switch pt {
			case vangogh_integration.SteamAppDetails:
				fallthrough
			case vangogh_integration.SteamAppNews:
				fallthrough
			case vangogh_integration.SteamDeckCompatibilityReport:
				fallthrough
			case vangogh_integration.SteamAppReviews:
				fallthrough
			case vangogh_integration.ProtonDbSummary:
				gogId := steamAppIdGogIds[id]
				idsTypes[gogId] = append(idsTypes[gogId], pt.String())
			default:
				idsTypes[id] = append(idsTypes[id], pt.String())
			}
		}
	}

	if err = rdx.BatchReplaceValues(vangogh_integration.TypesProperty, idsTypes); err != nil {
		return err
	}

	return nil
}

func getSteamAppIdGogIds(rdx redux.Readable) map[string]string {
	steamAppIdGogIds := make(map[string]string)

	for gogId := range rdx.Keys(vangogh_integration.SteamAppIdProperty) {
		if steamAppIds, ok := rdx.GetAllValues(vangogh_integration.SteamAppIdProperty, gogId); ok {
			for _, said := range steamAppIds {
				steamAppIdGogIds[said] = gogId
			}
		}
	}

	return steamAppIdGogIds
}
