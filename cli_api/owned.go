package cli_api

import (
	"github.com/arelate/gog_media"
	"github.com/arelate/vangogh_extracts"
	"github.com/arelate/vangogh_products"
	"github.com/arelate/vangogh_properties"
	"github.com/arelate/vangogh_values"
	"github.com/boggydigital/gost"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/vangogh/cli_api/output"
	"github.com/boggydigital/vangogh/cli_api/url_helpers"
	"net/url"
)

func OwnedHandler(u *url.URL) error {
	idSet, err := url_helpers.IdSet(u)
	if err != nil {
		return err
	}

	return Owned(idSet)
}

func Owned(idSet gost.StrSet) error {

	ownedSet := gost.NewStrSet()
	propSet := gost.NewStrSetWith(
		vangogh_properties.TitleProperty,
		vangogh_properties.SlugProperty,
		vangogh_properties.IncludesGamesProperty)

	exl, err := vangogh_extracts.NewList(propSet.All()...)
	if err != nil {
		return err
	}

	vrLicenceProducts, err := vangogh_values.NewReader(vangogh_products.LicenceProducts, gog_media.Game)
	if err != nil {
		return err
	}

	for _, id := range idSet.All() {

		if vrLicenceProducts.Contains(id) {
			ownedSet.Add(id)
			continue
		}

		includesGames, ok := exl.GetAllRaw(vangogh_properties.IncludesGamesProperty, id)
		if !ok || len(includesGames) == 0 {
			continue
		}

		ownAllIncludedGames := true
		for _, igId := range includesGames {
			ownAllIncludedGames = ownAllIncludedGames && vrLicenceProducts.Contains(igId)
			if !ownAllIncludedGames {
				break
			}
		}

		if ownAllIncludedGames {
			ownedSet.Add(id)
		}
	}

	noa := nod.Begin(" not owned:")
	itp, err := output.Items(idSet.Except(ownedSet), nil, []string{vangogh_properties.TitleProperty}, exl)
	if err != nil {
		return noa.EndWithError(err)
	}
	noa.EndWithSummary(itp)

	oa := nod.Begin(" owned:")
	itp, err = output.Items(ownedSet.All(), nil, []string{vangogh_properties.TitleProperty}, exl)
	if err != nil {
		return oa.EndWithError(err)
	}
	oa.EndWithSummary(itp)

	return nil
}
