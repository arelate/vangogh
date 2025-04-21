package shared_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"slices"
	"strings"
)

var demoTitleSuffixes = []string{
	"demo", "prologue",
}

func reduceDemo(rdx redux.Writeable) error {

	rda := nod.Begin(" reducing %s...", vangogh_integration.IsDemoProperty)
	defer rda.Done()

	if err := rdx.MustHave(
		vangogh_integration.TitleProperty,
		vangogh_integration.StoreTagsProperty,
		vangogh_integration.IsFreeProperty,
		vangogh_integration.IsDemoProperty); err != nil {
		return err
	}

	isDemo := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.TitleProperty) {

		if storeTags, ok := rdx.GetAllValues(vangogh_integration.StoreTagsProperty, id); ok {
			if slices.Contains(storeTags, "Demo") {
				isDemo[id] = []string{vangogh_integration.TrueValue}
				continue
			}
		}

		if isFree, sure := rdx.GetLastVal(vangogh_integration.IsFreeProperty, id); sure && isFree == vangogh_integration.TrueValue {
			if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
				if titleParts := strings.Split(strings.ToLower(title), " "); len(titleParts) > 1 {

					lastTitlePart := titleParts[len(titleParts)-1]
					if slices.Contains(demoTitleSuffixes, lastTitlePart) {
						isDemo[id] = []string{vangogh_integration.TrueValue}
						continue
					}

					if len(titleParts) > 2 {
						lastTwoTitleParts := strings.Join([]string{titleParts[len(titleParts)-2], titleParts[len(titleParts)-1]}, " ")
						if slices.Contains(demoTitleSuffixes, lastTwoTitleParts) {
							isDemo[id] = []string{vangogh_integration.TrueValue}
							continue
						}
					}
				}
			}
		}

		isDemo[id] = []string{vangogh_integration.FalseValue}
	}

	return rdx.BatchReplaceValues(vangogh_integration.IsDemoProperty, isDemo)
}
