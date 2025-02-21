package reductions

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
)

func Types() error {

	ta := nod.Begin(" %s...", vangogh_integration.TypesProperty)
	defer ta.Done()

	idsTypes := make(map[string][]string)

	for pt := range vangogh_integration.AllProductTypes() {

		vr, err := vangogh_integration.NewProductReader(pt)
		if err != nil {
			return err
		}

		for id := range vr.Keys() {

			if idsTypes[id] == nil {
				idsTypes[id] = make([]string, 0)
			}

			idsTypes[id] = append(idsTypes[id], pt.String())
		}
	}

	typesEx, err := vangogh_integration.NewReduxWriter(vangogh_integration.TypesProperty)
	if err != nil {
		return err
	}

	if err := typesEx.BatchReplaceValues(vangogh_integration.TypesProperty, idsTypes); err != nil {
		return err
	}

	return nil
}
