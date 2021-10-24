package itemize

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_urls"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
)

type imageExtractsGetter struct {
	imageType    vangogh_images.ImageType
	extractsList *vangogh_extracts.ExtractsList
}

func NewImageExtractsGetter(
	it vangogh_images.ImageType,
	exl *vangogh_extracts.ExtractsList) *imageExtractsGetter {
	return &imageExtractsGetter{
		imageType:    it,
		extractsList: exl,
	}
}

func (ieg *imageExtractsGetter) GetImageIds(id string) ([]string, bool) {
	return ieg.extractsList.GetAll(vangogh_properties.FromImageType(ieg.imageType), id)
}

func MissingLocalImages(
	it vangogh_images.ImageType,
	exl *vangogh_extracts.ExtractsList,
	localImageIds gost.StrSet) (gost.StrSet, error) {

	all := exl.All(vangogh_properties.FromImageType(it))

	if localImageIds == nil {
		var err error
		if localImageIds, err = vangogh_urls.LocalImageIds(); err != nil {
			return nil, err
		}
	}

	ieg := NewImageExtractsGetter(it, exl)

	mlia := nod.NewProgress(" itemizing local images (%s)...", it)
	defer mlia.EndWithResult("done")

	return missingLocalFiles(all, localImageIds, ieg.GetImageIds, nil, mlia)
}
