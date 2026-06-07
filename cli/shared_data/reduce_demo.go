package shared_data

import (
	"slices"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

var demoTitleSuffixes = []string{
	"demo", "prologue",
}

func reduceDemo(rdx redux.Writeable) error {

	rda := nod.Begin(" reducing %s...", vangogh_integration.GogIsDemoProperty)
	defer rda.Done()

	if err := rdx.MustHave(
		vangogh_integration.GogTitleProperty,
		vangogh_integration.GogStoreTagsProperty,
		vangogh_integration.GogIsFreeProperty,
		vangogh_integration.GogIsDemoProperty); err != nil {
		return err
	}

	isDemo := make(map[string][]string)

	for id := range rdx.Keys(vangogh_integration.GogTitleProperty) {

		if storeTags, ok := rdx.GetAllValues(vangogh_integration.GogStoreTagsProperty, id); ok {
			if slices.Contains(storeTags, "Demo") {
				isDemo[id] = []string{vangogh_integration.TrueValue}
				continue
			}
		}

		if isFree, sure := rdx.GetLastVal(vangogh_integration.GogIsFreeProperty, id); sure && isFree == vangogh_integration.TrueValue {
			if title, ok := rdx.GetLastVal(vangogh_integration.GogTitleProperty, id); ok {
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

	return rdx.BatchReplaceValues(vangogh_integration.GogIsDemoProperty, isDemo)
}
