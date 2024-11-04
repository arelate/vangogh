package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"image"
	"image/color"
	_ "image/jpeg"
	"net/url"
	"os"
	"strconv"
	"time"
)

func DehydrateHandler(u *url.URL) error {
	idSet, err := vangogh_local_data.IdSetFromUrl(u)
	if err != nil {
		return err
	}

	return Dehydrate(
		idSet,
		vangogh_local_data.ImageTypesFromUrl(u),
		vangogh_local_data.FlagFromUrl(u, "force"))
}

func Dehydrate(
	idSet map[string]bool,
	its []vangogh_local_data.ImageType,
	force bool) error {

	di := nod.NewProgress("dehydrating images...")
	defer di.End()

	properties := make([]string, 0, len(its)*2)
	for _, it := range its {
		properties = append(properties,
			vangogh_local_data.ImageTypeDehydratedProperty(it),
			vangogh_local_data.ImageTypeDehydratedModifiedProperty(it),
			vangogh_local_data.ImageTypeRepColorProperty(it))
	}

	rdx, err := imageTypesReduxAssets(properties, its)
	if err != nil {
		return di.EndWithError(err)
	}

	if len(idSet) == 0 {
		for _, it := range its {
			if !vangogh_local_data.IsImageTypeDehydrationSupported(it) {
				continue
			}

			asset := vangogh_local_data.PropertyFromImageType(it)
			dehydratedProperty := vangogh_local_data.ImageTypeDehydratedProperty(it)
			repColorProperty := vangogh_local_data.ImageTypeRepColorProperty(it)

			for _, id := range rdx.Keys(asset) {
				if rdx.HasKey(dehydratedProperty, id) &&
					rdx.HasKey(repColorProperty, id) &&
					!force {
					continue
				}
				idSet[id] = true
			}
		}
	}

	di.TotalInt(len(idSet) * len(its))

	plt := issa.ColorPalette()

	for _, it := range its {

		if !vangogh_local_data.IsImageTypeDehydrationSupported(it) {
			continue
		}

		asset := vangogh_local_data.PropertyFromImageType(it)

		dehydratedImages := make(map[string][]string)
		dehydratedImageModified := make(map[string][]string)
		repColors := make(map[string][]string)

		for id := range idSet {

			imageId, ok := rdx.GetLastVal(asset, id)
			if !ok {
				continue
			}

			alip, err := vangogh_local_data.AbsLocalImagePath(imageId)
			if err != nil {
				return di.EndWithError(err)
			}

			if dhi, rc, err := dehydrateImageRepColor(alip, plt); err == nil {
				dehydratedImages[id] = []string{dhi}
				repColors[id] = []string{rc}
				dehydratedImageModified[id] = []string{strconv.FormatInt(time.Now().Unix(), 10)}
			} else {
				nod.Log(err.Error())
			}

			di.Increment()
		}

		dehydratedProperty := vangogh_local_data.ImageTypeDehydratedProperty(it)
		if err := rdx.BatchReplaceValues(dehydratedProperty, dehydratedImages); err != nil {
			return di.EndWithError(err)
		}

		dehydratedModifiedProperty := vangogh_local_data.ImageTypeDehydratedModifiedProperty(it)
		if err := rdx.BatchReplaceValues(dehydratedModifiedProperty, dehydratedImageModified); err != nil {
			return di.EndWithError(err)
		}
	}

	di.EndWithResult("done")

	return nil
}

func dehydrateImageRepColor(absImagePath string, plt color.Palette) (string, string, error) {
	dhi, rc := "", ""

	fi, err := os.Open(absImagePath)
	if err != nil {
		return dhi, rc, err
	}
	defer fi.Close()

	img, _, err := image.Decode(fi)
	if err != nil {
		return dhi, rc, err
	}

	gif := issa.GIFImage(img, plt, issa.DefaultSampling)

	dhi, err = issa.DehydrateColor(gif)
	if err != nil {
		return dhi, rc, err
	}

	return dhi, issa.ColorHex(issa.RepColor(gif)), nil
}
