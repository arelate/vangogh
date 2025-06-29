package pcgw_data

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wikipedia_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/pathways"
	"github.com/boggydigital/redux"
	"maps"
	"net/url"
	"slices"
	"strconv"
	"strings"
)

func GetRaw(pcgwGogIds map[string][]string, force bool) error {

	gra := nod.NewProgress("getting %s...", vangogh_integration.PcgwRaw)
	defer gra.Done()

	rawDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.PcgwRaw)
	if err != nil {
		return err
	}

	kvRaw, err := kevlar.New(rawDir, kevlar.TxtExt)
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

		if err = reduceRawProduct(gogIds, pcgwPageId, kvRaw, rawReductions, rdx); err != nil {
			return err
		}

		rra.Increment()
	}

	return shared_data.WriteReductions(rdx, rawReductions)
}

var newValuesOnlyProperties = []string{
	vangogh_integration.SteamAppIdProperty,
	vangogh_integration.WebsiteProperty,
	vangogh_integration.OpenCriticMedianScoreProperty,
	vangogh_integration.MetacriticScoreProperty,
}

func reduceRawProduct(gogIds []string, pcgwPageId string, kvRaw kevlar.KeyValues, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

	rcRaw, err := kvRaw.Get(pcgwPageId)
	if err != nil {
		return err
	}
	defer rcRaw.Close()

	infoboxLines, err := wikipedia_integration.FilterInfoboxLines(rcRaw)
	if err != nil {
		return err
	}

	propertyLines := filterPropertyLines(infoboxLines...)

	propertyValues := parsePropertyValues(propertyLines)

	for property := range piv {

		for _, gogId := range gogIds {

			if slices.Contains(newValuesOnlyProperties, property) && rdx.HasKey(property, gogId) {
				continue
			}

			if values, ok := propertyValues[property]; ok && shared_data.IsNotEmpty(values...) {
				piv[property][gogId] = values
			}
		}

	}

	return nil
}

const infoboxGameRowReceptionPfx = "{{Infobox game/row/reception"

var ignoredPrefixes = []string{
	"|steam appid side",
}

var prefixedProperties = map[string]string{
	"|steam appid":   vangogh_integration.SteamAppIdProperty,
	"|hltb":          vangogh_integration.HltbIdProperty,
	"|igdb":          vangogh_integration.IgdbIdProperty,
	"|strategywiki":  vangogh_integration.StrategyWikiIdProperty,
	"|mobygames":     vangogh_integration.MobyGamesIdProperty,
	"|wikipedia":     vangogh_integration.WikipediaIdProperty,
	"|winehq":        vangogh_integration.WineHQIdProperty,
	"|official site": vangogh_integration.WebsiteProperty,

	"{{mm}} [https://vndb.org/": vangogh_integration.VndbIdProperty,

	infoboxGameRowReceptionPfx + "|Metacritic": vangogh_integration.MetacriticIdProperty,
	infoboxGameRowReceptionPfx + "|OpenCritic": vangogh_integration.OpenCriticIdProperty,
	infoboxGameRowReceptionPfx + "|IGDB":       vangogh_integration.IgdbIdProperty,

	"{{Infobox game/row/engine": vangogh_integration.EnginesProperty,
}

func filterPropertyLines(infoboxLines ...string) map[string][]string {

	propertyLines := make(map[string][]string)

	for _, line := range infoboxLines {

		ignoreLine := false
		for _, ip := range ignoredPrefixes {
			if strings.HasPrefix(line, ip) {
				ignoreLine = true
				break
			}
		}

		if ignoreLine {
			continue
		}

		for prefix, property := range prefixedProperties {
			if strings.HasPrefix(line, prefix) {
				propertyLines[property] = append(propertyLines[property], line)
				break
			}
		}

	}

	return propertyLines
}

func parsePropertyValues(propertyLines map[string][]string) map[string][]string {

	propertyValues := make(map[string][]string)

	for property, lines := range propertyLines {
		var parsedLines []string
		switch property {
		case vangogh_integration.SteamAppIdProperty:
			fallthrough
		case vangogh_integration.HltbIdProperty:
			fallthrough
		case vangogh_integration.StrategyWikiIdProperty:
			fallthrough
		case vangogh_integration.MobyGamesIdProperty:
			fallthrough
		case vangogh_integration.WikipediaIdProperty:
			fallthrough
		case vangogh_integration.WineHQIdProperty:
			fallthrough
		case vangogh_integration.WebsiteProperty:
			parsedLines = parseInfoboxPropertyLines(lines)
		case vangogh_integration.VndbIdProperty:
			parsedLines = parseVndbLines(lines)
		case vangogh_integration.EnginesProperty:
			for _, line := range lines {
				name, build := parseInfoboxEngineLine(line)
				if name != "" {
					propertyValues[vangogh_integration.EnginesProperty] =
						append(propertyValues[vangogh_integration.EnginesProperty], name)
				}
				if build != "" {
					propertyValues[vangogh_integration.EnginesBuildsProperty] =
						append(propertyValues[vangogh_integration.EnginesBuildsProperty], build)
				}
			}
		case vangogh_integration.IgdbIdProperty:
			if infoboxParsedLines := parseInfoboxPropertyLines(lines); len(infoboxParsedLines) > 0 {
				propertyValues[vangogh_integration.IgdbIdProperty] = infoboxParsedLines
			}

			for _, line := range lines {
				if id, _ := parseReceptionLines(line); id != "" {
					if slices.Contains(propertyValues[vangogh_integration.IgdbIdProperty], id) {
						continue
					}
					propertyValues[vangogh_integration.IgdbIdProperty] =
						append(propertyValues[vangogh_integration.IgdbIdProperty], id)
				}
			}
		case vangogh_integration.MetacriticIdProperty:
			for _, line := range lines {
				id, score := parseReceptionLines(line)
				if id != "" {
					propertyValues[vangogh_integration.MetacriticIdProperty] =
						append(propertyValues[vangogh_integration.MetacriticIdProperty], id)
				}
				if score != "" {
					propertyValues[vangogh_integration.MetacriticScoreProperty] =
						append(propertyValues[vangogh_integration.MetacriticScoreProperty], score)
				}
			}
		case vangogh_integration.OpenCriticIdProperty:
			for _, line := range lines {
				idSlug, score := parseReceptionLines(line)
				if idSlug != "" {
					if id, slug, ok := strings.Cut(idSlug, "/"); ok {
						propertyValues[vangogh_integration.OpenCriticIdProperty] = []string{id}
						propertyValues[vangogh_integration.OpenCriticSlugProperty] = []string{slug}

					}
				}
				if score != "" {
					propertyValues[vangogh_integration.OpenCriticMedianScoreProperty] = []string{score}
				}
			}
		}

		if len(parsedLines) > 0 {
			propertyValues[property] = parsedLines
		}
	}

	return propertyValues
}

func parseInfoboxPropertyLines(lines []string) []string {

	parsedLines := make([]string, 0, len(lines))

	for _, line := range lines {
		if _, value, ok := strings.Cut(line, "="); ok {
			if tsv := strings.TrimSpace(value); tsv != "" {
				parsedLines = append(parsedLines, tsv)
			}
		}
	}

	return parsedLines
}

func parseReceptionLines(line string) (id string, score string) {
	if parts := strings.Split(line, "|"); len(parts) == 4 && parts[0] == infoboxGameRowReceptionPfx {
		id = parts[2]
		if id == "link" {
			id = ""
		}

		// only valid numbers will be considered for a rating
		score = strings.TrimSuffix(parts[3], "}}")
		if score == "tbd" || score == "rating" {
			score = ""
		} else if ri, err := strconv.ParseInt(score, 10, 32); err != nil && ri <= 0 {
			score = ""
		}
	}
	return id, score
}

func parseVndbLines(lines []string) []string {
	parsedLines := make([]string, 0, len(lines))

	for _, line := range lines {
		if parts := strings.Split(line, " "); len(parts) >= 2 {
			if vndbUrl := strings.TrimPrefix(parts[1], "["); vndbUrl != "" {
				if u, err := url.Parse(vndbUrl); err == nil && u.Host == "vndb.org" {
					parsedLines = append(parsedLines, strings.TrimPrefix(u.Path, "/"))
				}
			}
		}
	}

	return parsedLines
}

func parseInfoboxEngineLine(line string) (name string, build string) {
	for _, part := range strings.Split(line, "|") {
		if strings.HasPrefix(part, "name=") {
			name = strings.TrimPrefix(part, "name=")
			name = strings.TrimSuffix(name, "}}")
		}
		if strings.HasPrefix(part, "build=") {
			build = strings.TrimPrefix(part, "build=")
			build = strings.TrimSuffix(build, "}}")
		}
	}
	return name, build
}
