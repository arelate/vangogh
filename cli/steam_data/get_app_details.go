package steam_data

import (
	"encoding/json/v2"
	"errors"
	"iter"
	"maps"
	"strconv"

	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

func GetAppDetails(steamGogIds map[string][]string, force bool) error {

	gada := nod.NewProgress("getting %s...", vangogh_integration.SteamAppDetails)
	defer gada.Done()

	appDetailsDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamAppDetails)

	kvAppDetails, err := kevlar.New(appDetailsDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gada.TotalInt(len(steamGogIds))

	if err = fetch.Items(maps.Keys(steamGogIds), reqs.SteamAppDetails(), kvAppDetails, gada, force); err != nil {
		return err
	}

	return ReduceAppDetails(maps.Keys(steamGogIds), kvAppDetails)
}

func ReduceAppDetails(steamAppIds iter.Seq[string], kvAppDetails kevlar.KeyValues) error {

	dataType := vangogh_integration.SteamAppDetails

	rada := nod.NewProgress(" reducing %s...", dataType)
	defer rada.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.SteamAppDetailsProperties()...)
	if err != nil {
		return err
	}

	appDetailsReductions := shared_data.InitReductions(vangogh_integration.SteamAppDetailsProperties()...)

	for steamAppId := range steamAppIds {
		if !kvAppDetails.Has(steamAppId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + steamAppId))
			rada.Increment()
			continue
		}

		if err = reduceAppDetailsProduct(steamAppId, kvAppDetails, appDetailsReductions); err != nil {
			return err
		}

		rada.Increment()
	}

	return shared_data.WriteReductions(rdx, appDetailsReductions)
}

func reduceAppDetailsProduct(steamAppId string, kvAppDetails kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcSteamAppDetailsResponse, err := kvAppDetails.Get(steamAppId)
	if err != nil {
		return err
	}
	defer rcSteamAppDetailsResponse.Close()

	var sadr steam_integration.AppDetailsResponse
	if err = json.UnmarshalRead(rcSteamAppDetailsResponse, &sadr); err != nil {
		return err
	}

	var metacriticId string
	var metacriticScore int

	ad := sadr.GetAppDetails()

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.SteamRequiredAgeProperty:
			values = []string{strconv.FormatInt(int64(ad.GetRequiredAge()), 10)}
		case vangogh_integration.SteamControllerSupportProperty:
			values = []string{ad.GetControllerSupport()}
		case vangogh_integration.SteamShortDescriptionProperty:
			values = []string{ad.GetShortDescription()}
		case vangogh_integration.SteamWebsiteProperty:
			values = []string{ad.GetWebsite()}
		case vangogh_integration.MetacriticScoreProperty:

			metacriticScore = ad.GetMetacriticScore()

			if metacriticId != "" {
				piv[property][metacriticId] = []string{strconv.FormatInt(int64(metacriticScore), 10)}
			}
			continue
		case vangogh_integration.SteamMetacriticIdProperty:
			metacriticId, err = ad.GetMetacriticId()
			if err != nil {
				return err
			}

			if metacriticScore > 0 {
				piv[property][steamAppId] = []string{metacriticId}
			}
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
			piv[property][steamAppId] = values
		}

	}

	return nil
}
