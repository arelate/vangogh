package cmd

import (
	"fmt"
	"github.com/arelate/gog_types"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_types"
	"github.com/arelate/vangogh_urls"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/froth"
)

func List(ids []string, pt vangogh_types.ProductType, mt gog_types.Media) error {

	memoriesDst, err := vangogh_urls.MemoriesUrl(pt, mt)
	if err != nil {
		return err
	}

	titleMemStash, err := froth.NewStash(memoriesDst, vangogh_properties.TitleProperty)
	if err != nil {
		return err
	}

	vr, err := vangogh_values.NewReader(pt, mt)
	if err != nil {
		return err
	}

	if len(ids) == 0 {
		ids = vr.All()
	}

	for _, id := range ids {
		title, ok := titleMemStash.Get(id)
		if !ok {
			continue
		}

		fmt.Println(id, title)
	}

	return nil
}
