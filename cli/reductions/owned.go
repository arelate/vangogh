package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kvas"
	"github.com/boggydigital/nod"
)

func CheckOwnership(idSet map[string]bool, rxa kvas.ReduxAssets) (map[string]bool, error) {

	ownedSet := make(map[string]bool)

	if err := rxa.IsSupported(
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

		if val, ok := rxa.GetFirstVal(vangogh_local_data.OwnedProperty, id); ok && val == "true" {
			ownedSet[id] = true
			continue
		}

		if vrLicenceProducts.Has(id) {
			ownedSet[id] = true
			continue
		}

		includesGames, _ := rxa.GetAllUnchangedValues(vangogh_local_data.IncludesGamesProperty, id)

		// check if _all_ included games are owned
		ownAllIncludedGames := len(includesGames) > 0
		for _, igId := range includesGames {
			val, ok := rxa.GetFirstVal(vangogh_local_data.OwnedProperty, igId)
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
		if isIncludedByIsOwned(id, rxa, vrLicenceProducts) {
			ownedSet[id] = true
			continue
		}

	}

	return ownedSet, nil
}

func isIncludedByIsOwned(id string, rxa kvas.ReduxAssets, vrLicenceProducts *vangogh_local_data.ValueReader) bool {
	if iibg, ok := rxa.GetAllUnchangedValues(vangogh_local_data.IsIncludedByGamesProperty, id); !ok {
		return false
	} else {
		for _, aid := range iibg {
			if vrLicenceProducts.Has(aid) {
				return true
			}
			if isIncludedByIsOwned(aid, rxa, vrLicenceProducts) {
				return true
			}
		}
	}
	return false
}

func Owned() error {

	oa := nod.Begin(" %s...", vangogh_local_data.OwnedProperty)
	defer oa.End()

	rxa, err := vangogh_local_data.ConnectReduxAssets(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.OwnedProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.IncludesGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty)
	if err != nil {
		return oa.EndWithError(err)
	}

	idSet := make(map[string]bool)
	for _, id := range rxa.Keys(vangogh_local_data.TitleProperty) {
		idSet[id] = true
	}

	owned, err := CheckOwnership(idSet, rxa)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownedRdx := make(map[string][]string)

	for _, id := range rxa.Keys(vangogh_local_data.TitleProperty) {
		if _, ok := owned[id]; ok {
			ownedRdx[id] = []string{"true"}
		} else {
			ownedRdx[id] = []string{"false"}
		}
	}

	if err := rxa.BatchReplaceValues(vangogh_local_data.OwnedProperty, ownedRdx); err != nil {
		return oa.EndWithError(err)
	}

	oa.EndWithResult("done")

	return nil
}
