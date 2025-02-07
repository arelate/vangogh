package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"net/url"
	"strconv"
	"time"
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
	defer di.End()

	properties := make([]string, 0, len(its)*2)
	for _, it := range its {
		properties = append(properties,
			vangogh_integration.ImageTypeDehydratedProperty(it),
			vangogh_integration.ImageTypeDehydratedModifiedProperty(it),
			vangogh_integration.ImageTypeRepColorProperty(it))
	}

	rdx, err := imageTypesReduxAssets(properties, its)
	if err != nil {
		return di.EndWithError(err)
	}

	if len(ids) == 0 {
		for _, it := range its {
			if !vangogh_integration.IsImageTypeDehydrationSupported(it) {
				continue
			}

			asset := vangogh_integration.PropertyFromImageType(it)
			dehydratedProperty := vangogh_integration.ImageTypeDehydratedProperty(it)
			repColorProperty := vangogh_integration.ImageTypeRepColorProperty(it)

			for id := range rdx.Keys(asset) {
				if rdx.HasKey(dehydratedProperty, id) &&
					rdx.HasKey(repColorProperty, id) &&
					!force {
					continue
				}
				ids = append(ids, id)
			}
		}
	}

	di.TotalInt(len(ids) * len(its))

	for _, it := range its {

		if !vangogh_integration.IsImageTypeDehydrationSupported(it) {
			continue
		}

		asset := vangogh_integration.PropertyFromImageType(it)

		dehydratedImages := make(map[string][]string)
		dehydratedImageModified := make(map[string][]string)
		repColors := make(map[string][]string)

		for _, id := range ids {

			imageId, ok := rdx.GetLastVal(asset, id)
			if !ok {
				continue
			}

			alip, err := vangogh_integration.AbsLocalImagePath(imageId)
			if err != nil {
				return di.EndWithError(err)
			}

			if dhi, rc, err := issa.DehydrateImageRepColor(alip); err == nil {
				dehydratedImages[id] = []string{dhi}
				repColors[id] = []string{rc}
				dehydratedImageModified[id] = []string{strconv.FormatInt(time.Now().Unix(), 10)}
			} else {
				nod.Log(err.Error())
			}

			di.Increment()
		}

		dehydratedProperty := vangogh_integration.ImageTypeDehydratedProperty(it)
		if err := rdx.BatchReplaceValues(dehydratedProperty, dehydratedImages); err != nil {
			return di.EndWithError(err)
		}

		dehydratedModifiedProperty := vangogh_integration.ImageTypeDehydratedModifiedProperty(it)
		if err := rdx.BatchReplaceValues(dehydratedModifiedProperty, dehydratedImageModified); err != nil {
			return di.EndWithError(err)
		}

		repColorProperty := vangogh_integration.ImageTypeRepColorProperty(it)
		if err := rdx.BatchReplaceValues(repColorProperty, repColors); err != nil {
			return di.EndWithError(err)
		}
	}

	di.EndWithResult("done")

	return nil
}
