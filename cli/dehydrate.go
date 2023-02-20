package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"image"
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

	rxa, err := imageTypesReduxAssets(
		[]string{vangogh_local_data.DehydratedImageProperty, vangogh_local_data.DehydratedImageModifiedProperty},
		its)
	if err != nil {
		return di.EndWithError(err)
	}

	if missing {
		for _, it := range its {
			if !vangogh_local_data.IsImageTypeDehydrationSupported(it) {
				continue
			}

			asset := vangogh_local_data.PropertyFromImageType(it)

			for _, id := range rxa.Keys(asset) {
				if !rxa.HasKey(vangogh_local_data.DehydratedImageProperty, id) {
					idSet[id] = true
				}
			}
		}
	}

	di.TotalInt(len(idSet))

	dehydratedImages := make(map[string][]string)
	dehydratedImageModified := make(map[string][]string)

	plt := issa.StdPalette()

	for id := range idSet {

		for _, it := range its {
			if !vangogh_local_data.IsImageTypeDehydrationSupported(it) {
				continue
			}

			samples := vangogh_local_data.ImageTypeDehydrationSamples(it)
			if samples < 0 {
				continue
			}

			asset := vangogh_local_data.PropertyFromImageType(it)
			imageId, ok := rxa.GetFirstVal(asset, id)
			if !ok {
				continue
			}

			fi, err := os.Open(vangogh_local_data.AbsLocalImagePath(imageId))
			if err != nil {
				return di.EndWithError(err)
			}

			img, _, err := image.Decode(fi)
			if err != nil {
				return di.EndWithError(err)
			}

			gif := issa.GIFImage(img, plt, samples)

			dhi, err := issa.Dehydrate(gif)
			if err != nil {
				return di.EndWithError(err)
			}

			dehydratedImages[id] = []string{dhi}
			dehydratedImageModified[id] = []string{strconv.FormatInt(time.Now().Unix(), 10)}
		}

		di.Increment()
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.DehydratedImageProperty, dehydratedImages); err != nil {
		return di.EndWithError(err)
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.DehydratedImageModifiedProperty, dehydratedImageModified); err != nil {
		return di.EndWithError(err)
	}

	di.EndWithResult("done")

	return nil
}
