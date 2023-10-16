package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/nod"
	"os"
	"strconv"
	"time"
)

// StaleDehydrations needs to be in cli to avoid import cycle
func StaleDehydrations(fix bool) error {

	if err := staleDehydrationsImageType(
		vangogh_local_data.ImageProperty,
		vangogh_local_data.DehydratedImageModifiedProperty, fix); err != nil {
		return err
	}

	if err := staleDehydrationsImageType(
		vangogh_local_data.VerticalImageProperty,
		vangogh_local_data.DehydratedVerticalImageModifiedProperty, fix); err != nil {
		return err
	}

	return nil
}

func staleDehydrationsImageType(imageProperty, dimProperty string, fix bool) error {

	sdia := nod.NewProgress("checking stale dehydrations for %s...", imageProperty)
	defer sdia.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(imageProperty, dimProperty)
	if err != nil {
		return err
	}

	staleIds := make(map[string]bool)

	ids := rxa.Keys(imageProperty)
	sdia.TotalInt(len(ids))

	for _, id := range ids {
		if imageId, ok := rxa.GetFirstVal(imageProperty, id); ok {
			imagePath, err := vangogh_local_data.AbsLocalImagePath(imageId)
			if err != nil {
				return sdia.EndWithError(err)
			}
			if stat, err := os.Stat(imagePath); err == nil {
				if dimStr, ok := rxa.GetFirstVal(dimProperty, id); ok {
					if dim, err := strconv.ParseInt(dimStr, 10, 64); err == nil {
						dimTime := time.Unix(dim, 0)
						imt := stat.ModTime()
						if imt.After(dimTime) {
							staleIds[id] = true
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
			imageType := vangogh_local_data.ImageTypeFromProperty(imageProperty)
			return Dehydrate(staleIds, []vangogh_local_data.ImageType{imageType}, false)
		}

	}

	return nil
}
