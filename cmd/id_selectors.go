package cmd

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_images"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/internal"
)

type idSelectors struct {
	ids       []string
	slugs     []string
	fromStdin bool
}

type imageIdSelectors struct {
	idSelectors
	imageType vangogh_images.ImageType
}

func SetFromSelectors(idSel idSelectors) (idSet gost.StrSet, err error) {

	idSet = gost.NewStrSetWith(idSel.ids...)

	if idSel.fromStdin {
		stdinIds, err := internal.ReadStdinIds()
		if err != nil {
			return idSet, err
		}
		idSet.Add(stdinIds...)
	}

	var exl *vangogh_extracts.ExtractsList

	if len(idSel.slugs) > 0 {
		exl, err = vangogh_extracts.NewList(vangogh_properties.SlugProperty)
		if err != nil {
			return idSet, err
		}
	}

	for _, slug := range idSel.slugs {
		if slug != "" {
			slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
			idSet.Add(slugIds...)
		}
	}

	return idSet, err
}
