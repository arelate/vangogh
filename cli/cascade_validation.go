package cli

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"net/url"
)

func CascadeValidationHandler(u *url.URL) error {
	return CascadeValidation()
}

func CascadeValidation() error {

	cva := nod.NewProgress("cascading validation...")
	defer cva.End()

	rdx, err := vangogh_integration.NewReduxWriter(
		//vangogh_integration.OwnedProperty,
		vangogh_integration.IsRequiredByGamesProperty,
		vangogh_integration.IsIncludedByGamesProperty,
		vangogh_integration.ProductTypeProperty,
		vangogh_integration.ProductValidationResultProperty)
	if err != nil {
		return err
	}

	cascadedResults := make(map[string][]string)

	cva.TotalInt(rdx.Len(vangogh_integration.ProductValidationResultProperty))

	for id := range rdx.Keys(vangogh_integration.ProductValidationResultProperty) {

		vr, _ := rdx.GetAllValues(vangogh_integration.ProductValidationResultProperty, id)

		for _, ri := range filterOwnedRelated(rdx, vangogh_integration.IsRequiredByGamesProperty, id) {
			cascadedResults[ri] = vr
		}

		for _, ii := range filterOwnedRelated(rdx, vangogh_integration.IsIncludedByGamesProperty, id) {
			cascadedResults[ii] = vr
		}

		cva.Increment()
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.ProductValidationResultProperty, cascadedResults); err != nil {
		return cva.EndWithError(err)
	}

	return nil
}

func filterOwnedRelated(rdx redux.Readable, p, id string) []string {
	ownedRelated := make([]string, 0)
	if related, ok := rdx.GetAllValues(p, id); ok {
		for _, rid := range related {
			if own, ok := rdx.GetLastVal(vangogh_integration.LicencesProperty, rid); ok && own == vangogh_integration.TrueValue {
				ownedRelated = append(ownedRelated, rid)
			}
		}
	}
	return ownedRelated
}
