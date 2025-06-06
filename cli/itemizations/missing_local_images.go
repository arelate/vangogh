package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

type imageReduxGetter struct {
	imageType vangogh_integration.ImageType
	rdx       redux.Readable
}

func NewImageReduxGetter(
	it vangogh_integration.ImageType,
	rdx redux.Readable) *imageReduxGetter {
	return &imageReduxGetter{
		imageType: it,
		rdx:       rdx,
	}
}

func (ieg *imageReduxGetter) GetImageIds(id string) ([]string, bool) {
	return ieg.rdx.GetAllValues(vangogh_integration.PropertyFromImageType(ieg.imageType), id)
}

func MissingLocalImages(
	it vangogh_integration.ImageType,
	rdx redux.Readable,
	localImageIds map[string]any) (map[string]bool, error) {

	all := rdx.Keys(vangogh_integration.PropertyFromImageType(it))

	if localImageIds == nil {
		var err error
		if localImageIds, err = vangogh_integration.LocalImageIds(); err != nil {
			return map[string]bool{}, err
		}
	}

	ieg := NewImageReduxGetter(it, rdx)

	mlia := nod.Begin(" itemizing local images (%s)...", it)
	defer mlia.Done()

	return missingLocalFiles(all, localImageIds, ieg.GetImageIds, nil)
}
