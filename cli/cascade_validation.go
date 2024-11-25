package cli

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"net/url"
)

func CascadeValidationHandler(u *url.URL) error {
	return CascadeValidation()
}

func CascadeValidation() error {

	cva := nod.NewProgress("cascading validation...")
	defer cva.End()

	_, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.OwnedProperty,
		vangogh_local_data.IsRequiredByGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty,
		vangogh_local_data.ProductTypeProperty,
		vangogh_local_data.ManualUrlStatusProperty)
	//vangogh_local_data.ValidationResultProperty)
	if err != nil {
		return err
	}

	//cascadedResults := make(map[string][]string)

	ids := make([]string, 0) //rdx.Keys(vangogh_local_data.ValidationResultProperty)

	cva.TotalInt(len(ids))

	for range ids {

		//vr, _ := rdx.GetAllValues(vangogh_local_data.ValidationResultProperty, id)
		//
		//for _, ri := range filterOwnedRelated(rdx, vangogh_local_data.IsRequiredByGamesProperty, id) {
		//	cascadedResults[ri] = vr
		//}
		//
		//for _, ii := range filterOwnedRelated(rdx, vangogh_local_data.IsIncludedByGamesProperty, id) {
		//	cascadedResults[ii] = vr
		//}

		cva.Increment()
	}

	//if err := rdx.BatchReplaceValues(vangogh_local_data.ValidationResultProperty, cascadedResults); err != nil {
	//	return err
	//}

	return nil
}

func filterOwnedRelated(rdx kevlar.ReadableRedux, p, id string) []string {
	ownedRelated := make([]string, 0)
	if related, ok := rdx.GetAllValues(p, id); ok {
		for _, rid := range related {
			if own, ok := rdx.GetLastVal(vangogh_local_data.OwnedProperty, rid); ok && own == "true" {
				ownedRelated = append(ownedRelated, rid)
			}
		}
	}
	return ownedRelated
}
