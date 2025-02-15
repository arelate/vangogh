package gog_data

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"strconv"
)

func GetGamesDbGogProducts(hc *http.Client, userAccessToken string, since int64, force bool) error {

	ggdpa := nod.NewProgress("getting %s...", vangogh_integration.GamesDbGogProducts)
	defer ggdpa.Done()

	if force {
		since = -1
	}

	gamesDbGogProductsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.GamesDbGogProducts)
	if err != nil {
		return err
	}

	kvGamesDbGogProducts, err := kevlar.New(gamesDbGogProductsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	ids, err := getCatalogPagesProducts(since)
	if err != nil {
		return err
	}

	// TODO: Save errors and dates and don't request them again for 30 days
	if itemErrs := fetch.Items(gog_integration.GamesDbGogExternalReleaseUrl, hc, http.MethodGet, userAccessToken, kvGamesDbGogProducts, ggdpa, ids...); len(itemErrs) > 0 {
		return fmt.Errorf("get %s errors: %v", vangogh_integration.ApiProductsV2, itemErrs)
	}

	return reduceGamesDbGogProducts(kvGamesDbGogProducts, since)
}

func reduceGamesDbGogProducts(kvGamesDbGogProducts kevlar.KeyValues, since int64) error {
	rgdgpa := nod.Begin(" reducing %s...", vangogh_integration.GamesDbGogProducts)
	defer rgdgpa.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	gamesDbGogProductsProperties := []string{
		vangogh_integration.SteamAppIdProperty,
		vangogh_integration.VideoIdProperty,
		vangogh_integration.AggregatedRatingProperty,
		vangogh_integration.ThemesProperty,
		vangogh_integration.GameModesProperty,
	}

	rdx, err := redux.NewWriter(reduxDir, gamesDbGogProductsProperties...)
	if err != nil {
		return err
	}

	gamesDbGogProductsReductions := initReductions(gamesDbGogProductsProperties...)

	updatedGamesDbGogProducts := kvGamesDbGogProducts.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedGamesDbGogProducts {
		if err = reduceGamesDbGogProduct(id, kvGamesDbGogProducts, gamesDbGogProductsReductions); err != nil {
			return err
		}
	}

	return writeReductions(rdx, gamesDbGogProductsReductions)
}

func reduceGamesDbGogProduct(id string, kvGamesDbGogProducts kevlar.KeyValues, piv propertyIdValues) error {

	rcGamesDbGogProduct, err := kvGamesDbGogProducts.Get(id)
	if err != nil {
		return err
	}
	defer rcGamesDbGogProduct.Close()

	var gdgp gog_integration.GamesDbProduct
	if err = json.NewDecoder(rcGamesDbGogProduct).Decode(&gdgp); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.SteamAppIdProperty:
			for _, steamAppId := range gdgp.GetSteamAppIds() {
				values = append(values, strconv.FormatInt(int64(steamAppId), 10))
			}
		case vangogh_integration.VideoIdProperty:
			values = gdgp.GetVideoIds()
		case vangogh_integration.AggregatedRatingProperty:
			values = []string{strconv.FormatFloat(gdgp.GetAggregatedRating(), 'f', -1, 64)}
		case vangogh_integration.ThemesProperty:
			values = gdgp.GetThemes()
		case vangogh_integration.GameModesProperty:
			values = gdgp.GetGameModes()
		}

		piv[property][id] = values

	}

	return nil

}
