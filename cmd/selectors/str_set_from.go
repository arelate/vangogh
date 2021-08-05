package selectors

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/internal"
)

func StrSetFrom(idSel Id) (idSet gost.StrSet, err error) {

	idSet = gost.NewStrSetWith(idSel.Ids...)

	if idSel.FromStdin {
		stdinIds, err := internal.ReadStdinIds()
		if err != nil {
			return idSet, err
		}
		idSet.Add(stdinIds...)
	}

	var exl *vangogh_extracts.ExtractsList

	if len(idSel.Slugs) > 0 {
		exl, err = vangogh_extracts.NewList(vangogh_properties.SlugProperty)
		if err != nil {
			return idSet, err
		}
	}

	for _, slug := range idSel.Slugs {
		if slug != "" {
			slugIds := exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)
			idSet.Add(slugIds...)
		}
	}

	return idSet, err
}
