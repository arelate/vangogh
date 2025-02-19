package gog_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"net/http"
	"slices"
	"strconv"
)

func GetGamesDbGogProducts(hc *http.Client, uat string, since int64, force bool) error {

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

	catalogPagesProducts, err := shared_data.GetCatalogPagesProducts(since)
	if err != nil {
		return err
	}

	ggdpa.TotalInt(len(catalogPagesProducts))

	if err = fetch.Items(slices.Values(catalogPagesProducts), reqs.GamesDbGogProduct(hc, uat), kvGamesDbGogProducts, ggdpa); err != nil {
		return err
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

	gamesDbGogProductsReductions := shared_data.InitReductions(gamesDbGogProductsProperties...)

	updatedGamesDbGogProducts := kvGamesDbGogProducts.Since(since, kevlar.Create, kevlar.Update)

	for id := range updatedGamesDbGogProducts {
		if err = reduceGamesDbGogProduct(id, kvGamesDbGogProducts, gamesDbGogProductsReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, gamesDbGogProductsReductions)
}

func reduceGamesDbGogProduct(id string, kvGamesDbGogProducts kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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
