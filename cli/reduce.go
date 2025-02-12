package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/reductions"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"net/url"
	"strings"
)

func ReduceHandler(u *url.URL) error {
	var since int64
	if vangogh_integration.FlagFromUrl(u, "since-hours-ago") {
		var err error
		since, err = vangogh_integration.SinceFromUrl(u)
		if err != nil {
			return err
		}
	}
	return Reduce(
		since,
		vangogh_integration.PropertiesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "properties-only"))
}

func Reduce(since int64, properties []string, propertiesOnly bool) error {

	propSet := make(map[string]bool)
	for _, p := range properties {
		propSet[p] = true
	}

	if len(properties) == 0 {
		for _, rp := range vangogh_integration.ReduxProperties() {
			propSet[rp] = true
		}
	}

	//if !propertiesOnly {
	//	if propSet[vangogh_integration.LanguageNameProperty] ||
	//		propSet[vangogh_integration.NativeLanguageNameProperty] {
	//		//required for language-* properties reduction below
	//		propSet[vangogh_integration.LanguageCodeProperty] = true
	//	}
	//}

	ra := nod.Begin("reducing properties...")
	defer ra.EndWithResult("done")

	rdx, err := vangogh_integration.NewReduxWriter(maps.Keys(propSet)...)
	if err != nil {
		return err
	}

	for _, pt := range vangogh_integration.LocalProducts() {

		vr, err := vangogh_integration.NewProductReader(pt)
		if err != nil {
			return err
		}

		missingProps := vangogh_integration.SupportedPropertiesOnly(pt, maps.Keys(propSet))

		missingPropRedux := make(map[string]map[string][]string, 0)

		var modifiedIds []string
		if since > 0 {
			for id := range vr.Since(since, kevlar.Create, kevlar.Update) {
				modifiedIds = append(modifiedIds, id)
			}
		} else {
			for id := range vr.Keys() {
				modifiedIds = append(modifiedIds, id)
			}
		}

		pta := nod.Begin(" %s...", pt)

		for _, id := range modifiedIds {

			if len(missingProps) == 0 {
				continue
			}

			propValues, err := vangogh_integration.GetProperties(id, vr, missingProps)
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
				return err
			}
		}

		pta.EndWithResult("done")
	}

	if err := reductions.SteamAppId(since); err != nil {
		return err
	}

	if !propertiesOnly {
		////language-names are reduced separately from general pipeline,
		////given we'll be filling the blanks from api-products-v2 using
		////GetLanguages property that returns map[string]string
		//langCodeSet, err := reductions.GetLanguageCodes(rdx)
		//if err != nil {
		//	return err
		//}
		//
		//if err := reductions.LanguageNames(langCodeSet); err != nil {
		//	return err
		//}
		//
		//if err := reductions.NativeLanguageNames(langCodeSet); err != nil {
		//	return err
		//}

		//tag-names are reduced separately from other types,
		//given it is most convenient to reduce from account-pages
		if err := reductions.TagNames(); err != nil {
			return err
		}

		//orders are reduced separately from other types
		if err := reductions.Orders(since); err != nil {
			return err
		}

		if err := reductions.Types(); err != nil {
			return err
		}

		if err := reductions.Wishlisted(); err != nil {
			return err
		}

		if err := reductions.Owned(); err != nil {
			return err
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
