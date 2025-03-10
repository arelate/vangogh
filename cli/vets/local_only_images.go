package vets

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"maps"
	"os"
	"slices"
)

func LocalOnlyImages(fix bool) error {

	loia := nod.Begin("checking for local only images...")
	defer loia.Done()

	ilia := nod.Begin(" itemizing local images...")
	localImages, err := vangogh_integration.LocalImageIds()
	if err != nil {
		ilia.EndWithResult(err.Error())
		return err
	}
	ilia.Done()

	propSet := make(map[string]bool)
	for _, it := range vangogh_integration.AllImageTypes() {
		propSet[vangogh_integration.PropertyFromImageType(it)] = true
	}

	properties := slices.Collect(maps.Keys(propSet))

	rdx, err := vangogh_integration.NewReduxReader(properties...)
	if err != nil {
		return err
	}

	ieia := nod.NewProgress(" itemizing expected images...")
	totalProducts := 0
	for p := range propSet {
		for range rdx.Keys(p) {
			totalProducts++
		}
	}

	ieia.TotalInt(totalProducts)

	expectedImages := make(map[string]bool)
	for p := range propSet {
		for id := range rdx.Keys(p) {
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

	ieia.Done()

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

	aip, err := pathways.GetAbsDir(vangogh_integration.Images)
	if err != nil {
		return err
	}

	if fix && len(unexpectedImages) > 0 {
		floia := nod.NewProgress(" removing %d local only image(s)...", len(unexpectedImages))
		floia.TotalInt(len(unexpectedImages))

		for _, imageId := range unexpectedImages {
			absLocalImagePath, err := vangogh_integration.AbsLocalImagePath(imageId)
			if err != nil {
				return err
			}
			nod.Log("removing local only imageId=%s file=%s", imageId, absLocalImagePath)
			if err := vangogh_integration.MoveToRecycleBin(aip, absLocalImagePath); err != nil && !os.IsNotExist(err) {
				return err
			}
			floia.Increment()
		}
		floia.Done()
	}

	return nil
}
