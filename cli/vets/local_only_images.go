package vets

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"golang.org/x/exp/maps"
	"os"
)

func LocalOnlyImages(fix bool) error {

	loia := nod.Begin("checking for local only images...")
	defer loia.End()

	ilia := nod.Begin(" itemizing local images...")
	localImages, err := vangogh_local_data.LocalImageIds()
	if err != nil {
		ilia.End()
		return loia.EndWithError(err)
	}
	ilia.EndWithResult("done")

	propSet := make(map[string]bool)
	for _, it := range vangogh_local_data.AllImageTypes() {
		propSet[vangogh_local_data.PropertyFromImageType(it)] = true
	}

	rdx, err := vangogh_local_data.NewReduxReader(maps.Keys(propSet)...)
	if err != nil {
		return loia.EndWithError(err)
	}

	ieia := nod.NewProgress(" itemizing expected images...")
	totalProducts := 0
	for p := range propSet {
		totalProducts += len(rdx.Keys(p))
	}

	ieia.TotalInt(totalProducts)

	expectedImages := make(map[string]bool)
	for p := range propSet {
		for _, id := range rdx.Keys(p) {
			imageIds, ok := rdx.GetAllValues(p, id)
			if !ok {
				ieia.Increment()
				continue
			}
			for _, imageId := range imageIds {
				if imageId == "" {
					continue
				}
				expectedImages[imageId] = true
			}
			ieia.Increment()
		}
	}

	ieia.EndWithResult("done")

	unexpectedImages := make([]string, 0, len(expectedImages))
	for imageId := range localImages {
		if imageId == "" {
			continue
		}
		if !expectedImages[imageId] {
			unexpectedImages = append(unexpectedImages, imageId)
		}
	}

	loia.EndWithResult("found %d unexpected images", len(unexpectedImages))

	aip, err := pathways.GetAbsDir(vangogh_local_data.Images)
	if err != nil {
		return loia.EndWithError(err)
	}

	if fix && len(unexpectedImages) > 0 {
		floia := nod.NewProgress(" removing %d local only image(s)...", len(unexpectedImages))
		floia.TotalInt(len(unexpectedImages))

		for _, imageId := range unexpectedImages {
			absLocalImagePath, err := vangogh_local_data.AbsLocalImagePath(imageId)
			if err != nil {
				return floia.EndWithError(err)
			}
			nod.Log("removing local only imageId=%s file=%s", imageId, absLocalImagePath)
			if err := vangogh_local_data.MoveToRecycleBin(aip, absLocalImagePath); err != nil && !os.IsNotExist(err) {
				return floia.EndWithError(err)
			}
			floia.Increment()
		}
		floia.EndWithResult("done")
	}

	return nil
}
