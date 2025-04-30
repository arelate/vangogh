package steam_data

import (
	"encoding/json"
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"strconv"
)

func GetAppDetails(steamGogIds map[string][]string, force bool) error {

	gada := nod.NewProgress("getting %s...", vangogh_integration.SteamAppDetails)
	defer gada.Done()

	appDetailsDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppDetails)
	if err != nil {
		return err
	}

	kvAppDetails, err := kevlar.New(appDetailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gada.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppDetails(), kvAppDetails, gada, force); err != nil {
		return err
	}

	return ReduceAppDetails(steamGogIds, kvAppDetails)
}

func ReduceAppDetails(steamGogIds map[string][]string, kvAppDetails kevlar.KeyValues) error {

	dataType := vangogh_integration.SteamAppDetails

	rada := nod.NewProgress(" reducing %s...", dataType)
	defer rada.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.SteamAppDetailsProperties()...)
	if err != nil {
		return err
	}

	appDetailsReductions := shared_data.InitReductions(vangogh_integration.SteamAppDetailsProperties()...)

	rada.TotalInt(len(steamGogIds))

	for steamAppId, gogIds := range steamGogIds {
		if !kvAppDetails.Has(steamAppId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, steamAppId))
			rada.Increment()
			continue
		}

		if err = reduceAppDetailsProduct(gogIds, steamAppId, kvAppDetails, appDetailsReductions); err != nil {
			return err
		}

		rada.Increment()
	}

	return shared_data.WriteReductions(rdx, appDetailsReductions)
}

func reduceAppDetailsProduct(gogIds []string, steamAppId string, kvAppDetails kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcSteamAppDetailsResponse, err := kvAppDetails.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcSteamAppDetailsResponse.Close()

	var sadr steam_integration.AppDetailsResponse
	if err = json.NewDecoder(rcSteamAppDetailsResponse).Decode(&sadr); err != nil {
		return err
	}

	ad := sadr.GetAppDetails()

	for property := range piv {

		for _, gogId := range gogIds {
			var values []string

			switch property {
			case vangogh_integration.RequiredAgeProperty:
				values = []string{strconv.FormatInt(int64(ad.GetRequiredAge()), 10)}
			case vangogh_integration.ControllerSupportProperty:
				values = []string{ad.GetControllerSupport()}
			case vangogh_integration.ShortDescriptionProperty:
				values = []string{ad.GetShortDescription()}
			case vangogh_integration.WebsiteProperty:
				values = []string{ad.GetWebsite()}
			case vangogh_integration.MetacriticScoreProperty:
				values = []string{strconv.FormatInt(int64(ad.GetMetacriticScore()), 10)}
			case vangogh_integration.MetacriticIdProperty:
				mid, err := ad.GetMetacriticId()
				if err != nil {
					return err
				}

				values = []string{mid}
			case vangogh_integration.SteamCategoriesProperty:
				values = ad.GetCategories()
			case vangogh_integration.SteamGenresProperty:
				values = ad.GetGenres()
			case vangogh_integration.SteamSupportUrlProperty:
				values = []string{ad.GetSupportUrl()}
			case vangogh_integration.SteamSupportEmailProperty:
				values = []string{ad.GetSupportEmail()}
			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][gogId] = values
			}
		}

	}

	return nil
}
