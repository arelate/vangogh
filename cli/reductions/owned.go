package reductions

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"golang.org/x/exp/maps"
	"slices"
)

func CheckOwnership(ids []string, rdx kevlar.ReadableRedux) ([]string, error) {

	ownedSet := make(map[string]bool)

	if err := rdx.MustHave(
		vangogh_integration.SlugProperty,
		vangogh_integration.IncludesGamesProperty,
		vangogh_integration.IsIncludedByGamesProperty,
		vangogh_integration.OwnedProperty); err != nil {
		return nil, err
	}

	vrLicenceProducts, err := vangogh_integration.NewProductReader(vangogh_integration.LicenceProducts)
	if err != nil {
		return nil, err
	}

	for _, id := range ids {

		if val, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && val == "true" {
			ownedSet[id] = true
			continue
		}

		if vrLicenceProducts.Has(id) {
			ownedSet[id] = true
			continue
		}

		includesGames, _ := rdx.GetAllValues(vangogh_integration.IncludesGamesProperty, id)

		// check if all included games are owned
		ownAllIncludedGames := len(includesGames) > 0
		for _, igId := range includesGames {
			val, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, igId)
			has := vrLicenceProducts.Has(igId)

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

func isIncludedByIsOwned(id string, rdx kevlar.ReadableRedux, vrLicenceProducts *vangogh_integration.ProductReader) bool {
	if iibg, ok := rdx.GetAllValues(vangogh_integration.IsIncludedByGamesProperty, id); !ok {
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

	oa := nod.Begin(" %s...", vangogh_integration.OwnedProperty)
	defer oa.End()

	rdx, err := vangogh_integration.NewReduxWriter(
		vangogh_integration.TitleProperty,
		vangogh_integration.OwnedProperty,
		vangogh_integration.SlugProperty,
		vangogh_integration.IncludesGamesProperty,
		vangogh_integration.IsIncludedByGamesProperty)
	if err != nil {
		return oa.EndWithError(err)
	}

	ids := rdx.Keys(vangogh_integration.TitleProperty)
	owned, err := CheckOwnership(ids, rdx)
	if err != nil {
		return oa.EndWithError(err)
	}

	ownedRdx := make(map[string][]string)

	for _, id := range rdx.Keys(vangogh_integration.TitleProperty) {
		if slices.Contains(owned, id) {
			ownedRdx[id] = []string{"true"}
		} else {
			ownedRdx[id] = []string{"false"}
		}
	}

	if err := rdx.BatchReplaceValues(vangogh_integration.OwnedProperty, ownedRdx); err != nil {
		return oa.EndWithError(err)
	}

	oa.EndWithResult("done")

	return nil
}
