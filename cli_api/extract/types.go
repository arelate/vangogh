package extract

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/nod"
)

func Types(mt gog_media.Media) error {

	ta := nod.Begin(" %s...", vangogh_properties.TypesProperty)
	defer ta.End()

	idsTypes := make(map[string][]string)

	for _, pt := range vangogh_products.Local() {

		vr, err := vangogh_values.NewReader(pt, mt)
		if err != nil {
			return ta.EndWithError(err)
		}

		for _, id := range vr.All() {

			if idsTypes[id] == nil {
				idsTypes[id] = make([]string, 0)
			}

			idsTypes[id] = append(idsTypes[id], pt.String())
		}
	}

	typesEx, err := vangogh_extracts.NewList(vangogh_properties.TypesProperty)
	if err != nil {
		return ta.EndWithError(err)
	}

	if err := typesEx.SetMany(vangogh_properties.TypesProperty, idsTypes); err != nil {
		return ta.EndWithError(err)
	}

	ta.EndWithResult("done")

	return nil
}
