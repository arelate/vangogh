package pcgw_data

import (
	"bufio"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"io"
	"maps"
	"strings"
)

const (
	txtExt = ".txt"
)

func GetRaw(pcgwGogIds map[string][]string, force bool) error {

	gra := nod.NewProgress("getting %s...", vangogh_integration.PcgwRaw)
	defer gra.Done()

	rawDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwRaw)
	if err != nil {
		return err
	}

	kvRaw, err := kevlar.New(rawDir, txtExt)
	if err != nil {
		return err
	}

	gra.TotalInt(len(pcgwGogIds))

	if err = fetch.Items(maps.Keys(pcgwGogIds), reqs.PcgwRaw(), kvRaw, gra, force); err != nil {
		return err
	}

	return ReduceRaw(pcgwGogIds, kvRaw)
}

func ReduceRaw(pcgwGogIds map[string][]string, kvRaw kevlar.KeyValues) error {

	dataType := vangogh_integration.PcgwRaw

	rra := nod.NewProgress(" reducing %s...", dataType)
	defer rra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.PcgwRawProperties()...)
	if err != nil {
		return err
	}

	rawReductions := shared_data.InitReductions(vangogh_integration.PcgwRawProperties()...)

	rra.TotalInt(len(pcgwGogIds))

	for pcgwPageId, gogIds := range pcgwGogIds {
		if !kvRaw.Has(pcgwPageId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, pcgwPageId))
			continue
		}

		if err = reduceRawProduct(gogIds, pcgwPageId, kvRaw, rawReductions); err != nil {
			return err
		}

		rra.Increment()
	}

	return shared_data.WriteReductions(rdx, rawReductions)
}

func reduceRawProduct(gogIds []string, pcgwPageId string, kvRaw kevlar.KeyValues, piv shared_data.PropertyIdValues) error {

	rcRaw, err := kvRaw.Get(pcgwPageId)
	if err != nil {
		return err
	}
	defer rcRaw.Close()

	//propertyLines, err := filterPropertyLines(rcRaw)
	//if err != nil {
	//	return err
	//}
	//
	//fmt.Println(propertyLines)

	//for property := range piv {
	//
	//for _, gogId := range gogIds {
	//
	//}
	//
	//}

	return nil
}

var prefixedProperties = map[string]string{

	"{{Infobox game/row/engine": vangogh_integration.EnginesProperty,

	"|steam appid":   vangogh_integration.SteamAppIdProperty,
	"|hltb":          vangogh_integration.HltbIdProperty,
	"|igdb":          vangogh_integration.IgdbIdProperty,
	"|strategywiki":  vangogh_integration.StrategyWikiIdProperty,
	"|mobygames":     vangogh_integration.MobyGamesIdProperty,
	"|wikipedia":     vangogh_integration.WikipediaIdProperty,
	"|winehq":        vangogh_integration.WineHQIdProperty,
	"|official site": vangogh_integration.WebsiteProperty,

	"{{mm}} [https://vndb.org/": vangogh_integration.VndbIdProperty,

	"{{Infobox game/row/reception|Metacritic": vangogh_integration.MetacriticIdProperty,
	"{{Infobox game/row/reception|OpenCritic": vangogh_integration.OpenCriticIdProperty,
	"{{Infobox game/row/reception|IGDB":       vangogh_integration.IgdbIdProperty,
}

func filterPropertyLines(rcRaw io.Reader) (map[string][]string, error) {

	propertyLines := make(map[string][]string)

	rawScanner := bufio.NewScanner(rcRaw)
	for rawScanner.Scan() {

		line := rawScanner.Text()

		for prefix, property := range prefixedProperties {
			if strings.HasPrefix(line, prefix) {
				propertyLines[property] = append(propertyLines[property], line)
				break
			}
		}

	}

	if err := rawScanner.Err(); err != nil {
		return nil, err
	}

	return propertyLines, nil
}
