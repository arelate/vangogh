package reductions

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"slices"
)

func CheckOwnership(ids []string, rdx kevlar.ReadableRedux) ([]string, error) {

	ownedSet := make(map[string]bool)

	if err := rdx.MustHave(
		vangogh_local_data.SlugProperty,
		vangogh_local_data.IncludesGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty,
		vangogh_local_data.OwnedProperty); err != nil {
		return nil, err
	}

	vrLicenceProducts, err := vangogh_local_data.NewProductReader(vangogh_local_data.LicenceProducts)
	if err != nil {
		return nil, err
	}

	for _, id := range ids {

		if val, ok := rdx.GetLastVal(vangogh_local_data.OwnedProperty, id); ok && val == "true" {
			ownedSet[id] = true
			continue
		}

		has, err := vrLicenceProducts.Has(id)
		if err != nil {
			return nil, err
		}

		if has {
			ownedSet[id] = true
			continue
		}

		includesGames, _ := rdx.GetAllValues(vangogh_local_data.IncludesGamesProperty, id)

		// check if all included games are owned
		ownAllIncludedGames := len(includesGames) > 0
		for _, igId := range includesGames {
			val, ok := rdx.GetLastVal(vangogh_local_data.OwnedProperty, igId)
			has, err := vrLicenceProducts.Has(igId)
			if err != nil {
				return nil, err
			}

			ownAllIncludedGames = ownAllIncludedGames && (has || (ok && val == "true"))
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

	return maps.Keys(ownedSet), nil
}

func isIncludedByIsOwned(id string, rdx kevlar.ReadableRedux, vrLicenceProducts *vangogh_local_data.ProductReader) bool {
	if iibg, ok := rdx.GetAllValues(vangogh_local_data.IsIncludedByGamesProperty, id); !ok {
		return false
	} else {
		for _, aid := range iibg {
			has, err := vrLicenceProducts.Has(aid)
			if err != nil {
				return false
			}
			if has {
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

	rdx, err := vangogh_local_data.NewReduxWriter(
		vangogh_local_data.TitleProperty,
		vangogh_local_data.OwnedProperty,
		vangogh_local_data.SlugProperty,
		vangogh_local_data.IncludesGamesProperty,
		vangogh_local_data.IsIncludedByGamesProperty)
	if err != nil {
		return oa.EndWithError(err)
	}

	ids := rdx.Keys(vangogh_local_data.TitleProperty)
	owned, err := CheckOwnership(ids, rdx)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownedRdx := make(map[string][]string)

	for _, id := range rdx.Keys(vangogh_local_data.TitleProperty) {
		if slices.Contains(owned, id) {
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
