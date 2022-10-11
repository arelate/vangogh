package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
	"image"
	"os"
	"strconv"
	"time"
)

const vangoghSamplingRate = 20

func DehydrateImages(idSet map[string]bool, force bool) error {
	dia := nod.NewProgress("dehydrating images...")
	defer dia.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.ImageProperty,
		vangogh_local_data.DehydratedImageProperty,
		vangogh_local_data.DehydratedImageModifiedProperty,
		vangogh_local_data.MissingDehydratedImageProperty)

	if err != nil {
		return dia.EndWithError(err)
	}

	dehydratedImages := make(map[string][]string)
	dehydratedImagesModified := make(map[string][]string)

	if len(idSet) == 0 {
		idSet = make(map[string]bool)
		for _, id := range rxa.Keys(vangogh_local_data.ImageProperty) {
			idSet[id] = true
		}
	}

	for id := range idSet {
		if force {
			dehydratedImages[id] = nil
			continue
		}

		if dip, ok := rxa.GetFirstVal(vangogh_local_data.DehydratedImageProperty, id); !ok || dip == "" {
			dehydratedImages[id] = nil
		}
	}

	dia.TotalInt(len(dehydratedImages))

	for id := range dehydratedImages {

		if imageId, ok := rxa.GetFirstVal(vangogh_local_data.ImageProperty, id); ok {

			// skipping known problematic images, unless forcing dehydration
			if !force && rxa.HasVal(vangogh_local_data.MissingDehydratedImageProperty, id, imageId) {
				continue
			}

			absLocalImagePath := vangogh_local_data.AbsLocalImagePath(imageId)
			if fi, err := os.Open(absLocalImagePath); err == nil {
				if jpegImage, _, err := image.Decode(fi); err == nil {
					gifImage := issa.GIFImage(jpegImage, issa.StdPalette(), vangoghSamplingRate)

					if dhi, err := issa.Dehydrate(gifImage); err == nil {
						dehydratedImages[id] = []string{dhi}
						dehydratedImagesModified[id] = []string{strconv.FormatInt(time.Now().Unix(), 10)}
					} else {
						if werr := setError(id, imageId, rxa, dia, err); werr != nil {
							return dia.EndWithError(werr)
						}
					}
				} else {
					if werr := setError(id, imageId, rxa, dia, err); werr != nil {
						return dia.EndWithError(werr)
					}
				}
			} else if !os.IsNotExist(err) {
				if werr := setError(id, imageId, rxa, dia, err); werr != nil {
					return dia.EndWithError(werr)
				}
			}

		}
		dia.Increment()
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.DehydratedImageProperty, dehydratedImages); err != nil {
		return dia.EndWithError(err)
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.DehydratedImageModifiedProperty, dehydratedImagesModified); err != nil {
		return dia.EndWithError(err)
	}

	dia.EndWithResult("done")

	return nil
}

func setError(id, imageId string, rxa kvas.ReduxAssets, tpw nod.TotalProgressWriter, err error) error {

	if tpw != nil {
		tpw.Error(err)
	}
	return rxa.AddVal(vangogh_local_data.MissingDehydratedImageProperty, id, imageId)
}
