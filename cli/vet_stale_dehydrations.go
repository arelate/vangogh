package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"os"
	"strconv"
	"time"
)

// StaleDehydrations needs to be in cli to avoid import cycle
func StaleDehydrations(fix bool) error {

	if err := staleDehydrationsImageType(
		vangogh_integration.ImageProperty,
		vangogh_integration.DehydratedImageModifiedProperty, fix); err != nil {
		return err
	}

	if err := staleDehydrationsImageType(
		vangogh_integration.VerticalImageProperty,
		vangogh_integration.DehydratedVerticalImageModifiedProperty, fix); err != nil {
		return err
	}

	return nil
}

func staleDehydrationsImageType(imageProperty, dimProperty string, fix bool) error {

	sdia := nod.NewProgress("checking stale dehydrations for %s...", imageProperty)
	defer sdia.End()

	rdx, err := vangogh_integration.NewReduxReader(imageProperty, dimProperty)
	if err != nil {
		return err
	}

	staleIds := make([]string, 0)

	ids := rdx.Keys(imageProperty)
	sdia.TotalInt(len(ids))

	for _, id := range ids {
		if imageId, ok := rdx.GetLastVal(imageProperty, id); ok {
			imagePath, err := vangogh_integration.AbsLocalImagePath(imageId)
			if err != nil {
				return sdia.EndWithError(err)
			}
			if stat, err := os.Stat(imagePath); err == nil {
				if dimStr, ok := rdx.GetLastVal(dimProperty, id); ok {
					if dim, err := strconv.ParseInt(dimStr, 10, 64); err == nil {
						dimTime := time.Unix(dim, 0)
						imt := stat.ModTime()
						if imt.After(dimTime) {
							staleIds = append(staleIds, id)
						}
					}
				}
			}
		}
		sdia.Increment()
	}

	if len(staleIds) == 0 {
		sdia.EndWithResult("all good")
	} else {
		sdia.EndWithResult("found %d stale dehydrations", len(staleIds))
		if fix {
			imageType := vangogh_integration.ImageTypeFromProperty(imageProperty)
			return Dehydrate(staleIds, []vangogh_integration.ImageType{imageType}, false)
		}

	}

	return nil
}
