package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func reduceTypes(rdx redux.Writeable) error {

	rta := nod.Begin(" reducing %s...", vangogh_integration.TypesProperty)
	defer rta.Done()

	steamAppIdGogIds := GetSteamAppIdGogIds(rdx)
	pcgwPageIdsGogIds := GetPcgwPageIdGogIds(rdx)
	hltbIdsGogIds := GetHltbIdGogIds(rdx)
	openCriticGogIds := GetOpenCriticIdGogIds(rdx)

	idsTypes := make(map[string][]string)

	for pt := range vangogh_integration.AllProductTypes() {

		if pt == vangogh_integration.UnknownProductType {
			continue
		}

		ptDir, err := vangogh_integration.AbsProductTypeDir(pt)
		if err != nil {
			return err
		}

		ext := kevlar.JsonExt
		if pt == vangogh_integration.PcgwRaw {
			ext = ".txt"
		}

		kvPt, err := kevlar.New(ptDir, ext)
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
			case vangogh_integration.PcgwSteamPageId:
				fallthrough
			case vangogh_integration.ProtonDbSummary:
				for _, gogId := range steamAppIdGogIds[id] {
					idsTypes[gogId] = append(idsTypes[gogId], pt.String())
				}
			case vangogh_integration.PcgwEngine:
				fallthrough
			case vangogh_integration.PcgwExternalLinks:
				fallthrough
			case vangogh_integration.PcgwRaw:
				for _, gogId := range pcgwPageIdsGogIds[id] {
					idsTypes[gogId] = append(idsTypes[gogId], pt.String())
				}
			case vangogh_integration.HltbData:
				for _, gogId := range hltbIdsGogIds[id] {
					idsTypes[gogId] = append(idsTypes[gogId], pt.String())
				}
			case vangogh_integration.OpenCriticApiGame:
				for _, gogId := range openCriticGogIds[id] {
					idsTypes[gogId] = append(idsTypes[gogId], pt.String())
				}
			default:
				idsTypes[id] = append(idsTypes[id], pt.String())
			}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.TypesProperty, idsTypes); err != nil {
		return err
	}

	return nil
}

func GetSteamAppIdGogIds(rdx redux.Readable) map[string][]string {
	return externalIdsGogIds(vangogh_integration.SteamAppIdProperty, rdx)
}

func GetPcgwPageIdGogIds(rdx redux.Readable) map[string][]string {
	return externalIdsGogIds(vangogh_integration.PcgwPageIdProperty, rdx)
}

func GetHltbIdGogIds(rdx redux.Readable) map[string][]string {
	return externalIdsGogIds(vangogh_integration.HltbIdProperty, rdx)
}

func GetOpenCriticIdGogIds(rdx redux.Readable) map[string][]string {
	return externalIdsGogIds(vangogh_integration.OpenCriticIdProperty, rdx)
}

func externalIdsGogIds(externalIdProperty string, rdx redux.Readable) map[string][]string {

	externalIdGogIds := make(map[string][]string)

	for gogId := range rdx.Keys(externalIdProperty) {
		if externalIds, ok := rdx.GetAllValues(externalIdProperty, gogId); ok {
			for _, eid := range externalIds {
				externalIdGogIds[eid] = append(externalIdGogIds[eid], gogId)
			}
		}
	}

	return externalIdGogIds
}
