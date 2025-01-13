package itemizations

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
)

type imageReduxGetter struct {
	imageType vangogh_integration.ImageType
	rdx       kevlar.ReadableRedux
}

func NewImageReduxGetter(
	it vangogh_integration.ImageType,
	rdx kevlar.ReadableRedux) *imageReduxGetter {
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
	rdx kevlar.ReadableRedux,
	localImageIds map[string]bool) (map[string]bool, error) {

	all := rdx.Keys(vangogh_integration.PropertyFromImageType(it))

	if localImageIds == nil {
		var err error
		if localImageIds, err = vangogh_integration.LocalImageIds(); err != nil {
			return map[string]bool{}, err
		}
	}

	ieg := NewImageReduxGetter(it, rdx)

	mlia := nod.NewProgress(" itemizing local images (%s)...", it)
	defer mlia.EndWithResult("done")

	return missingLocalFiles(all, localImageIds, ieg.GetImageIds, nil, mlia)
}
