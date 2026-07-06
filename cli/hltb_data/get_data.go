package hltb_data

import (
	"encoding/json/v2"
	"errors"
	"iter"
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

	dataDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.HltbData)

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

	return ReduceData(maps.Keys(hltbGogIds), kvData)
}

func readBuildId() (string, error) {

	rootPageDir := vangogh_integration.AbsProductTypeDir(vangogh_integration.HltbRootPage)

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

func ReduceData(hltbIds iter.Seq[string], kvData kevlar.KeyValues) error {

	dataType := vangogh_integration.HltbData

	rda := nod.NewProgress(" reducing %s...", dataType)
	defer rda.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.HltbDataProperties()...)
	if err != nil {
		return err
	}

	dataReductions := shared_data.InitReductions(vangogh_integration.HltbDataProperties()...)

	for hltbId := range hltbIds {
		if !kvData.Has(hltbId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + hltbId))
			rda.Increment()
			continue
		}

		if err = reduceDataProduct(hltbId, kvData, dataReductions); err != nil {
			return err
		}

		rda.Increment()
	}

	return shared_data.WriteReductions(rdx, dataReductions)
}

func reduceDataProduct(hltbId string, kvData kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcData, err := kvData.Get(hltbId)
	if err != nil {
		return err
	}
	defer rcData.Close()

	var data hltb_integration.Data
	if err = json.UnmarshalRead(rcData, &data); err != nil {
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
		}

		if shared_data.IsNotEmpty(values...) {
			piv[property][hltbId] = values
		}

	}

	return nil

}
