package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
)

func CheckOwnership(idSet map[string]bool, rdx kvas.ReadableRedux) (map[string]bool, error) {

	ownedSet := make(map[string]bool)

	if err := rdx.MustHave(
		vangogh_local_data.SlugProperty,
		vangogh_local_data.IncludesGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty,
		vangogh_local_data.OwnedProperty); err != nil {
		return ownedSet, err
	}

	vrLicenceProducts, err := vangogh_local_data.NewReader(vangogh_local_data.LicenceProducts)
	if err != nil {
		return ownedSet, err
	}

	for id := range idSet {

		if val, ok := rdx.GetFirstVal(vangogh_local_data.OwnedProperty, id); ok && val == "true" {
			ownedSet[id] = true
			continue
		}

		if vrLicenceProducts.Has(id) {
			ownedSet[id] = true
			continue
		}

		includesGames, _ := rdx.GetAllValues(vangogh_local_data.IncludesGamesProperty, id)

		// check if all included games are owned
		ownAllIncludedGames := len(includesGames) > 0
		for _, igId := range includesGames {
			val, ok := rdx.GetFirstVal(vangogh_local_data.OwnedProperty, igId)
			ownAllIncludedGames = ownAllIncludedGames && (vrLicenceProducts.Has(igId) || (ok && val == "true"))
			if !ownAllIncludedGames {
				break
			}
		}

		if ownAllIncludedGames {
			ownedSet[id] = true
			continue
		}

		// check if _any_ is-included-by product is owned
		if isIncludedByIsOwned(id, rdx, vrLicenceProducts) {
			ownedSet[id] = true
			continue
		}

	}

	return ownedSet, nil
}

func isIncludedByIsOwned(id string, rdx kvas.ReadableRedux, vrLicenceProducts *vangogh_local_data.ValueReader) bool {
	if iibg, ok := rdx.GetAllValues(vangogh_local_data.IsIncludedByGamesProperty, id); !ok {
		return false
	} else {
		for _, aid := range iibg {
			if vrLicenceProducts.Has(aid) {
				return true
			}
			if isIncludedByIsOwned(aid, rdx, vrLicenceProducts) {
				return true
			}
		}
	}
	return false
}

func Owned() error {

	oa := nod.Begin(" %s...", vangogh_local_data.OwnedProperty)
	defer oa.End()

	rdx, err := vangogh_local_data.ReduxWriter(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.OwnedProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.IncludesGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty)
	if err != nil {
		return oa.EndWithError(err)
	}

	idSet := make(map[string]bool)
	for _, id := range rdx.Keys(vangogh_local_data.TitleProperty) {
		idSet[id] = true
	}

	owned, err := CheckOwnership(idSet, rdx)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownedRdx := make(map[string][]string)

	for _, id := range rdx.Keys(vangogh_local_data.TitleProperty) {
		if _, ok := owned[id]; ok {
			ownedRdx[id] = []string{"true"}
		} else {
			ownedRdx[id] = []string{"false"}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_local_data.OwnedProperty, ownedRdx); err != nil {
		return oa.EndWithError(err)
	}

	oa.EndWithResult("done")

	return nil
}
