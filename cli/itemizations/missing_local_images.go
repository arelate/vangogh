package itemizations

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
)

type imageReduxGetter struct {
	imageType vangogh_local_data.ImageType
	rdx       kvas.ReadableRedux
}

func NewImageReduxGetter(
	it vangogh_local_data.ImageType,
	rdx kvas.ReadableRedux) *imageReduxGetter {
	return &imageReduxGetter{
		imageType: it,
		rdx:       rdx,
	}
}

func (ieg *imageReduxGetter) GetImageIds(id string) ([]string, bool) {
	return ieg.rdx.GetAllValues(vangogh_local_data.PropertyFromImageType(ieg.imageType), id)
}

func MissingLocalImages(
	it vangogh_local_data.ImageType,
	rdx kvas.ReadableRedux,
	localImageIds map[string]bool) (map[string]bool, error) {

	all := rdx.Keys(vangogh_local_data.PropertyFromImageType(it))

	if localImageIds == nil {
		var err error
		if localImageIds, err = vangogh_local_data.LocalImageIds(); err != nil {
			return map[string]bool{}, err
		}
	}

	ieg := NewImageReduxGetter(it, rdx)

	mlia := nod.NewProgress(" itemizing local images (%s)...", it)
	defer mlia.EndWithResult("done")

	return missingLocalFiles(all, localImageIds, ieg.GetImageIds, nil, mlia)
}
