package cli

import (
	"github.com/arelate/vangogh/cli/reductions"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
	"strings"
)

func ReduceHandler(u *url.URL) error {
	var since int64
	if vangogh_local_data.FlagFromUrl(u, "since-hours-ago") {
		var err error
		since, err = vangogh_local_data.SinceFromUrl(u)
		if err != nil {
			return err
		}
	}
	return Reduce(
		since,
		vangogh_local_data.PropertiesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "properties-only"))
}

func Reduce(since int64, properties []string, propertiesOnly bool) error {

	propSet := make(map[string]bool)
	for _, p := range properties {
		propSet[p] = true
	}

	if len(properties) == 0 {
		for _, rp := range vangogh_local_data.ReduxProperties() {
			propSet[rp] = true
		}
	}

	if !propertiesOnly {
		if propSet[vangogh_local_data.LanguageNameProperty] ||
			propSet[vangogh_local_data.NativeLanguageNameProperty] {
			//required for language-* properties reduction below
			propSet[vangogh_local_data.LanguageCodeProperty] = true
		}
	}

	ra := nod.Begin("reducing properties...")
	defer ra.End()

	rdx, err := vangogh_local_data.ReduxWriter(maps.Keys(propSet)...)
	if err != nil {
		return ra.EndWithError(err)
	}

	for _, pt := range vangogh_local_data.LocalProducts() {

		vr, err := vangogh_local_data.NewReader(pt)
		if err != nil {
			return ra.EndWithError(err)
		}

		missingProps := vangogh_local_data.SupportedPropertiesOnly(pt, maps.Keys(propSet))

		missingPropRedux := make(kvas.IdReduxAssets, 0)

		var modifiedIds []string
		if since > 0 {
			modifiedIds = vr.ModifiedAfter(since, false)
		} else {
			modifiedIds = vr.Keys()
		}

		if len(modifiedIds) == 0 || len(missingProps) == 0 {
			continue
		}

		pta := nod.NewProgress(" %s...", pt)
		pta.TotalInt(len(modifiedIds))

		for _, id := range modifiedIds {

			if len(missingProps) == 0 {
				pta.Increment()
				continue
			}

			propValues, err := vangogh_local_data.GetProperties(id, vr, missingProps)
			if err != nil {
				pta.Error(err)
				continue
			}

			for prop, values := range propValues {
				if _, ok := missingPropRedux[prop]; !ok {
					missingPropRedux[prop] = make(map[string][]string, 0)
				}
				if trValues := stringsTrimSpace(values); len(trValues) > 0 {
					missingPropRedux[prop][id] = trValues
				}
			}

			pta.Increment()
		}

		for prop, redux := range missingPropRedux {

			//TODO: This seems like a good place to diff redux per id with existing values
			//and track additional values as a changelist
			//for id, values := range redux {
			//	exValues, ok := exl.GetAllRaw(prop, id)
			//	if !ok {
			//		fmt.Printf("NEW %s for %s %s: %v\n", prop, pt, id, values)
			//	}
			//	if len(values) != len(exValues) {
			//		fmt.Printf("CHANGED %s for %s %s: %v -> %v\n", prop, pt, id, exValues, values)
			//	}
			//}

			if err := rdx.BatchReplaceValues(prop, redux); err != nil {
				return pta.EndWithError(err)
			}
		}

		pta.EndWithResult("done")
	}

	if err := reductions.SteamAppId(since); err != nil {
		return ra.EndWithError(err)
	}

	if !propertiesOnly {
		//language-names are reduced separately from general pipeline,
		//given we'll be filling the blanks from api-products-v2 using
		//GetLanguages property that returns map[string]string
		langCodeSet, err := reductions.GetLanguageCodes(rdx)
		if err != nil {
			return ra.EndWithError(err)
		}

		if err := reductions.LanguageNames(langCodeSet); err != nil {
			return ra.EndWithError(err)
		}

		if err := reductions.NativeLanguageNames(langCodeSet); err != nil {
			return ra.EndWithError(err)
		}

		//tag-names are reduced separately from other types,
		//given it is most convenient to reduce from account-pages
		if err := reductions.TagNames(); err != nil {
			return ra.EndWithError(err)
		}

		//orders are reduced separately from other types
		if err := reductions.Orders(since); err != nil {
			return ra.EndWithError(err)
		}

		if err := reductions.Types(); err != nil {
			return ra.EndWithError(err)
		}

		if err := reductions.Wishlisted(); err != nil {
			return ra.EndWithError(err)
		}

		if err := reductions.Owned(); err != nil {
			return ra.EndWithError(err)
		}
	}

	return reductions.Cascade()
}

func stringsTrimSpace(stringsWithSpace []string) []string {
	trimmedStrings := make([]string, 0, len(stringsWithSpace))
	for _, str := range stringsWithSpace {
		tStr := strings.TrimSpace(str)
		if tStr == "" {
			continue
		}
		trimmedStrings = append(trimmedStrings, tStr)
	}
	return trimmedStrings
}
