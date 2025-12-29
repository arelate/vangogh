package hltb_data

import (
	"encoding/json"
	"errors"
	"maps"
	"strconv"

	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"golang.org/x/net/html"
)

func GetData(hltbGogIds map[string][]string, force bool) error {

	gda := nod.NewProgress("gettings %s...", vangogh_integration.HltbData)
	defer gda.Done()

	dataDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.HltbData)
	if err != nil {
		return err
	}

	kvData, err := kevlar.New(dataDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gda.TotalInt(len(hltbGogIds))

	buildId, err := readBuildId()
	if err != nil {
		return err
	}

	if err = fetch.Items(maps.Keys(hltbGogIds), reqs.HltbData(buildId), kvData, gda, force); err != nil {
		return err
	}

	return ReduceData(hltbGogIds, kvData)
}

func readBuildId() (string, error) {

	rootPageDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.HltbRootPage)
	if err != nil {
		return "", err
	}

	kvRootPage, err := kevlar.New(rootPageDir, kevlar.HtmlExt)
	if err != nil {
		return "", err
	}

	rootPageId := vangogh_integration.HltbRootPage.String()

	rcRootPage, err := kvRootPage.Get(rootPageId)
	if err != nil {
		return "", err
	}
	defer rcRootPage.Close()

	rootPageDoc, err := html.Parse(rcRootPage)
	if err != nil {
		return "", err
	}

	rootPage := &hltb_integration.RootPage{Doc: rootPageDoc}

	return rootPage.GetBuildId(), nil
}

func ReduceData(hltbGogIds map[string][]string, kvData kevlar.KeyValues) error {

	dataType := vangogh_integration.HltbData

	rda := nod.NewProgress(" reducing %s...", dataType)
	defer rda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.HltbDataProperties()...)
	if err != nil {
		return err
	}

	dataReductions := shared_data.InitReductions(vangogh_integration.HltbDataProperties()...)

	rda.TotalInt(len(hltbGogIds))

	for hltbId, gogIds := range hltbGogIds {
		if !kvData.Has(hltbId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + hltbId))
			rda.Increment()
			continue
		}

		if err = reduceDataProduct(gogIds, hltbId, kvData, dataReductions); err != nil {
			return err
		}

		rda.Increment()
	}

	return shared_data.WriteReductions(rdx, dataReductions)
}

func reduceDataProduct(gogIds []string, hltbId string, kvData kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcData, err := kvData.Get(hltbId)
	if err != nil {
		return err
	}
	defer rcData.Close()

	var data hltb_integration.Data
	if err = json.NewDecoder(rcData).Decode(&data); err != nil {
		return err
	}

	for property := range piv {

		for _, gogId := range gogIds {

			var values []string

			switch property {
			case vangogh_integration.HltbHoursToCompleteMainProperty:
				values = []string{data.GetHoursToCompleteMain()}
			case vangogh_integration.HltbHoursToCompletePlusProperty:
				values = []string{data.GetHoursToCompletePlus()}
			case vangogh_integration.HltbHoursToComplete100Property:
				values = []string{data.GetHoursToComplete100()}
			case vangogh_integration.HltbReviewScoreProperty:
				values = []string{strconv.FormatInt(int64(data.GetReviewScore()), 10)}
			case vangogh_integration.HltbGenresProperty:
				values = data.GetGenres()
			case vangogh_integration.HltbPlatformsProperty:
				values = data.GetPlatforms()
			case vangogh_integration.IGNWikiSlugProperty:
				values = []string{data.GetIgnWikiSlug()}
			}

			if shared_data.IsNotEmpty(values...) {
				piv[property][gogId] = values
			}

		}
	}

	return nil

}
