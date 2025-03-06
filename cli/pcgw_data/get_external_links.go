package pcgw_data

import (
	"encoding/json"
	"github.com/arelate/southern_light/pcgw_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
)

func GetExternalLinks(pcgwGogIds map[string]string) error {

	gea := nod.NewProgress("getting %s...", vangogh_integration.PcgwExternalLinks)
	defer gea.Done()

	externalLinksDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwExternalLinks)
	if err != nil {
		return err
	}

	kvExternalLinks, err := kevlar.New(externalLinksDir, kevlar.JsonExt)
	if err != nil {
		return err
	}

	gea.TotalInt(len(pcgwGogIds))

	if err = fetch.Items(maps.Keys(pcgwGogIds), reqs.PcgwExternalLinks(), kvExternalLinks, gea); err != nil {
		return err
	}

	return ReduceExternalLinks(pcgwGogIds, kvExternalLinks)
}

func ReduceExternalLinks(pcgwGogIds map[string]string, kvExternalLinks kevlar.KeyValues) error {

	rela := nod.Begin(" reducing %s...", vangogh_integration.PcgwExternalLinks)
	defer rela.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.PcgwExternalLinksProperties()...)
	if err != nil {
		return err
	}

	externalLinksReductions := shared_data.InitReductions(vangogh_integration.PcgwExternalLinksProperties()...)

	for pcgwPageId, gogId := range pcgwGogIds {
		if err = reduceExternalLinksProduct(gogId, pcgwPageId, kvExternalLinks, externalLinksReductions); err != nil {
			return err
		}
	}

	return shared_data.WriteReductions(rdx, externalLinksReductions)
}

func reduceExternalLinksProduct(gogId, pcgwPageId string, kvExternalLinks kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcExternalLinks, err := kvExternalLinks.Get(pcgwPageId)
	if err != nil {
		return err
	}
	defer rcExternalLinks.Close()

	var externalLinks pcgw_integration.ParseExternalLinks
	if err = json.NewDecoder(rcExternalLinks).Decode(&externalLinks); err != nil {
		return err
	}

	for property := range piv {

		var values []string

		switch property {
		case vangogh_integration.HltbIdProperty:
			values = []string{externalLinks.GetHltbId()}
		case vangogh_integration.IgdbIdProperty:
			values = []string{externalLinks.GetIgdbId()}
		case vangogh_integration.MobyGamesIdProperty:
			values = []string{externalLinks.GetMobyGamesId()}
		case vangogh_integration.VndbIdProperty:
			values = []string{externalLinks.GetVndbId()}
		case vangogh_integration.WikipediaIdProperty:
			values = []string{externalLinks.GetWikipediaId()}
		case vangogh_integration.StrategyWikiIdProperty:
			values = []string{externalLinks.GetStrategyWikiId()}
		}

		piv[property][gogId] = values

	}

	return nil
}
