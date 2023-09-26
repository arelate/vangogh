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
		vangogh_local_data.FlagFromUrl(u, "missing"))
}

func Dehydrate(
	idSet map[string]bool,
	its []vangogh_local_data.ImageType,
	missing bool) error {

	di := nod.NewProgress("dehydrating images...")
	defer di.End()

	properties := make([]string, 0, len(its)*2)
	for _, it := range its {
		properties = append(properties,
			vangogh_local_data.ImageTypeDehydratedProperty(it),
			vangogh_local_data.ImageTypeDehydratedModifiedProperty(it))
	}

	rxa, err := imageTypesReduxAssets(properties, its)
	if err != nil {
		return di.EndWithError(err)
	}

	if missing {
		for _, it := range its {
			if !vangogh_local_data.IsImageTypeDehydrationSupported(it) {
				continue
			}

			asset := vangogh_local_data.PropertyFromImageType(it)
			dehydratedProperty := vangogh_local_data.ImageTypeDehydratedProperty(it)

			for _, id := range rxa.Keys(asset) {
				if !rxa.HasKey(dehydratedProperty, id) {
					idSet[id] = true
				}
			}
		}
	}

	di.TotalInt(len(idSet) * len(its))

	plt := issa.StdPalette()

	for _, it := range its {

		if !vangogh_local_data.IsImageTypeDehydrationSupported(it) {
			continue
		}

		asset := vangogh_local_data.PropertyFromImageType(it)

		dehydratedImages := make(map[string][]string)
		dehydratedImageModified := make(map[string][]string)

		for id := range idSet {

			imageId, ok := rxa.GetFirstVal(asset, id)
			if !ok {
				continue
			}

			alip, err := vangogh_local_data.AbsLocalImagePath(imageId)
			if err != nil {
				return di.EndWithError(err)
			}

			if dhi, err := dehydrateImage(alip, plt); err == nil {
				dehydratedImages[id] = []string{dhi}
				dehydratedImageModified[id] = []string{strconv.FormatInt(time.Now().Unix(), 10)}
			} else {
				nod.Log(err.Error())
			}

			di.Increment()
		}

		dehydratedProperty := vangogh_local_data.ImageTypeDehydratedProperty(it)
		if err := rxa.BatchReplaceValues(dehydratedProperty, dehydratedImages); err != nil {
			return di.EndWithError(err)
		}

		dehydratedModifiedProperty := vangogh_local_data.ImageTypeDehydratedModifiedProperty(it)
		if err := rxa.BatchReplaceValues(dehydratedModifiedProperty, dehydratedImageModified); err != nil {
			return di.EndWithError(err)
		}
	}

	di.EndWithResult("done")

	return nil
}

func dehydrateImage(absImagePath string, plt color.Palette) (string, error) {
	dhi := ""

	fi, err := os.Open(absImagePath)
	if err != nil {
		return dhi, err
	}
	defer fi.Close()

	img, _, err := image.Decode(fi)
	if err != nil {
		return dhi, err
	}

	gif := issa.GIFImage(img, plt, issa.DefaultDownSampling)

	return issa.Dehydrate(gif)
}
