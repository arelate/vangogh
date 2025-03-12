package hltb_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/hltb_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"golang.org/x/net/html"
	"slices"
	"strconv"
)

func GetData(since int64, force bool) error {

	gda := nod.NewProgress("gettings %s...", vangogh_integration.HltbData)
	defer gda.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewReader(reduxDir, vangogh_integration.HltbIdProperty)
	if err != nil {
		return err
	}

	dataDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.HltbData)
	if err != nil {
		return err
	}

	kvData, err := kevlar.New(dataDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	hltbIds := getHltbIds(rdx)

	gda.TotalInt(len(hltbIds))

	buildId, err := readBuildId()
	if err != nil {
		return err
	}

	if err = fetch.Items(slices.Values(hltbIds), reqs.HltbData(buildId), kvData, gda, force); err != nil {
		return err
	}

	return ReduceData(kvData, since)
}

func getHltbIds(rdx redux.Readable) []string {
	var ids []string

	for gogId := range rdx.Keys(vangogh_integration.HltbIdProperty) {
		if hltbIds, ok := rdx.GetAllValues(vangogh_integration.HltbIdProperty, gogId); ok {
			ids = append(ids, hltbIds...)
		}
	}

	return ids
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

func ReduceData(kvData kevlar.KeyValues, since int64) error {

	rda := nod.Begin(" reducing %s...", vangogh_integration.HltbData)
	defer rda.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.HltbDataProperties()...)
	if err != nil {
		return err
	}

	dataReductions := shared_data.InitReductions(vangogh_integration.HltbDataProperties()...)

	updatedData := kvData.Since(since, kevlar.Create, kevlar.Update)

	for hltbId := range updatedData {
		if matches := rdx.Match(map[string][]string{vangogh_integration.HltbIdProperty: {hltbId}}, redux.FullMatch); matches != nil {
			for gogId := range matches {
				if err = reduceDataProduct(gogId, hltbId, kvData, dataReductions); err != nil {
					return err
				}
			}
		}
	}

	return shared_data.WriteReductions(rdx, dataReductions)
}

func reduceDataProduct(gogId, hltbId string, kvData kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

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

		piv[property][gogId] = values

	}

	return nil

}
