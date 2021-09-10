package url_helpers

import (
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_properties"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/vangogh/cmd/lines"
	"net/url"
)

func SlugIds(exl *vangogh_extracts.ExtractsList, slugs []string) (slugId gost.StrSet, err error) {

	if exl == nil && len(slugs) > 0 {
		exl, err = vangogh_extracts.NewList(vangogh_properties.SlugProperty)
		if err != nil {
			return nil, err
		}
	}

	if exl != nil {
		if err := exl.AssertSupport(vangogh_properties.SlugProperty); err != nil {
			return nil, err
		}
	}

	idSet := gost.NewStrSet()
	for _, slug := range slugs {
		if slug != "" && exl != nil {
			idSet.Add(exl.Search(map[string][]string{vangogh_properties.SlugProperty: {slug}}, true)...)
		}
	}

	return idSet, nil
}

func IdSet(u *url.URL) (idSet gost.StrSet, err error) {

	idSet = gost.NewStrSetWith(Values(u, "id")...)

	if Flag(u, "read-ids") {
		pipedIds, err := lines.ReadPipedIds()
		if err != nil {
			return idSet, err
		}
		idSet.AddSet(pipedIds)
	}

	slugs := Values(u, "slug")

	slugIds, err := SlugIds(nil, slugs)
	if err != nil {
		return idSet, err
	}
	idSet.AddSet(slugIds)

	return idSet, err
}
