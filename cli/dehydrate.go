package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"net/url"
)

func DehydrateHandler(u *url.URL) error {
	ids, err := vangogh_integration.IdsFromUrl(u)
	if err != nil {
		return err
	}

	return Dehydrate(
		ids,
		vangogh_integration.ImageTypesFromUrl(u),
		vangogh_integration.FlagFromUrl(u, "force"))
}

func Dehydrate(
	ids []string,
	its []vangogh_integration.ImageType,
	force bool) error {

	di := nod.NewProgress("dehydrating images...")
	defer di.Done()

	dehydratedProperties := []string{vangogh_integration.DehydratedImageProperty, vangogh_integration.RepColorProperty}

	rdx, err := imageTypesReduxAssets(dehydratedProperties, its)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		for _, it := range its {

			imageProperty := vangogh_integration.PropertyFromImageType(it)

			for id := range rdx.Keys(imageProperty) {

				imageId, ok := rdx.GetLastVal(imageProperty, id)
				if !ok || imageId == "" {
					continue
				}

				if rdx.HasKey(vangogh_integration.DehydratedImageProperty, imageId) &&
					rdx.HasKey(vangogh_integration.RepColorProperty, imageId) &&
					!force {
					continue
				}
				ids = append(ids, id)
			}
		}
	}

	di.TotalInt(len(ids) * len(its))

	for _, it := range its {

		imageProperty := vangogh_integration.PropertyFromImageType(it)

		dehydratedImages := make(map[string][]string)
		repColors := make(map[string][]string)

		for _, id := range ids {

			imageId, ok := rdx.GetLastVal(imageProperty, id)
			if !ok || imageId == "" {
				continue
			}

			alip, err := vangogh_integration.AbsLocalImagePath(imageId)
			if err != nil {
				return err
			}

			if dhi, rc, err := issa.DehydrateImageRepColor(alip); err == nil {
				dehydratedImages[imageId] = []string{dhi}
				repColors[imageId] = []string{rc}
			} else {
				nod.LogError(err)
			}

			di.Increment()
		}

		if err = rdx.BatchReplaceValues(vangogh_integration.DehydratedImageProperty, dehydratedImages); err != nil {
			return err
		}

		if err = rdx.BatchReplaceValues(vangogh_integration.RepColorProperty, repColors); err != nil {
			return err
		}
	}

	return nil
}
