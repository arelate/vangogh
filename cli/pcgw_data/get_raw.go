package pcgw_data

import (
	"errors"
	"maps"
	"net/url"
	"slices"
	"strconv"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/southern_light/wikipedia_integration"
	"github.com/arelate/vangogh/cli/fetch"
	"github.com/arelate/vangogh/cli/reqs"
	"github.com/arelate/vangogh/cli/shared_data"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
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

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(),
		vangogh_integration.PcgwRawProperties()...)
	if err != nil {
		return err
	}

	rawReductions := shared_data.InitReductions(vangogh_integration.PcgwRawProperties()...)

	rra.TotalInt(len(pcgwGogIds))

	for pcgwPageId, gogIds := range pcgwGogIds {
		if !kvRaw.Has(pcgwPageId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + pcgwPageId))
			continue
		}

		if err = reduceRawProduct(gogIds, pcgwPageId, kvRaw, rawReductions, rdx); err != nil {
			return err
		}

		rra.Increment()
	}

	return shared_data.WriteReductions(rdx, rawReductions)
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

		switch property {
		case vangogh_integration.MetacriticScoreProperty:
			if mids := propertyValues[vangogh_integration.GogMetacriticIdProperty]; len(mids) > 0 {
				for _, metacriticId := range mids {
					if values, ok := propertyValues[property]; ok && shared_data.IsNotEmpty(values...) {
						piv[property][metacriticId] = values
					}
				}
			}
			continue
		case vangogh_integration.OpenCriticSlugProperty:
			if ocids := propertyValues[vangogh_integration.GogOpenCriticIdProperty]; len(ocids) > 0 {
				for _, opencriticId := range ocids {
					if values, ok := propertyValues[property]; ok && shared_data.IsNotEmpty(values...) {
						piv[property][opencriticId] = values
					}
				}
			}
			continue
		case vangogh_integration.PcgwEnginesProperty:
			fallthrough
		case vangogh_integration.PcgwEnginesBuildsProperty:
			if values, ok := propertyValues[property]; ok && shared_data.IsNotEmpty(values...) {
				piv[property][pcgwPageId] = values
			}
			continue
		}

		for _, gogId := range gogIds {

			switch property {
			case vangogh_integration.WebsiteProperty:
				if rdx.HasKey(property, gogId) {
					continue
				}

				if values, ok := propertyValues[property]; ok && shared_data.IsNotEmpty(values...) {
					piv[property][gogId] = values
				}
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
	"|steam appid":   vangogh_integration.GogSteamAppIdProperty,
	"|hltb":          vangogh_integration.GogHltbIdProperty,
	"|igdb":          vangogh_integration.GogIgdbIdProperty,
	"|strategywiki":  vangogh_integration.GogStrategyWikiIdProperty,
	"|mobygames":     vangogh_integration.GogMobyGamesIdProperty,
	"|wikipedia":     vangogh_integration.GogWikipediaIdProperty,
	"|winehq":        vangogh_integration.GogWineHqIdProperty,
	"|official site": vangogh_integration.WebsiteProperty,

	"{{mm}} [https://vndb.org/": vangogh_integration.GogVndbIdProperty,

	infoboxGameRowReceptionPfx + "|Metacritic": vangogh_integration.GogMetacriticIdProperty,
	infoboxGameRowReceptionPfx + "|OpenCritic": vangogh_integration.GogOpenCriticIdProperty,
	infoboxGameRowReceptionPfx + "|IGDB":       vangogh_integration.GogIgdbIdProperty,

	"{{Infobox game/row/engine": vangogh_integration.PcgwEnginesProperty,
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
		case vangogh_integration.GogSteamAppIdProperty:
			fallthrough
		case vangogh_integration.GogHltbIdProperty:
			fallthrough
		case vangogh_integration.GogStrategyWikiIdProperty:
			fallthrough
		case vangogh_integration.GogMobyGamesIdProperty:
			fallthrough
		case vangogh_integration.GogWikipediaIdProperty:
			fallthrough
		case vangogh_integration.GogWineHqIdProperty:
			fallthrough
		case vangogh_integration.WebsiteProperty:
			parsedLines = parseInfoboxPropertyLines(lines)
		case vangogh_integration.GogVndbIdProperty:
			parsedLines = parseVndbLines(lines)
		case vangogh_integration.PcgwEnginesProperty:
			for _, line := range lines {
				name, build := parseInfoboxEngineLine(line)
				if name != "" {
					propertyValues[vangogh_integration.PcgwEnginesProperty] =
						append(propertyValues[vangogh_integration.PcgwEnginesProperty], name)
				}
				if build != "" {
					propertyValues[vangogh_integration.PcgwEnginesBuildsProperty] =
						append(propertyValues[vangogh_integration.PcgwEnginesBuildsProperty], build)
				}
			}
		case vangogh_integration.GogIgdbIdProperty:
			if infoboxParsedLines := parseInfoboxPropertyLines(lines); len(infoboxParsedLines) > 0 {
				propertyValues[vangogh_integration.GogIgdbIdProperty] = infoboxParsedLines
			}

			for _, line := range lines {
				if id, _ := parseReceptionLines(line); id != "" {
					if slices.Contains(propertyValues[vangogh_integration.GogIgdbIdProperty], id) {
						continue
					}
					propertyValues[vangogh_integration.GogIgdbIdProperty] =
						append(propertyValues[vangogh_integration.GogIgdbIdProperty], id)
				}
			}
		case vangogh_integration.GogMetacriticIdProperty:
			for _, line := range lines {
				id, score := parseReceptionLines(line)
				if id != "" {
					propertyValues[vangogh_integration.GogMetacriticIdProperty] =
						append(propertyValues[vangogh_integration.GogMetacriticIdProperty], id)
				}
				if score != "" {
					propertyValues[vangogh_integration.MetacriticScoreProperty] =
						append(propertyValues[vangogh_integration.MetacriticScoreProperty], score)
				}
			}
		case vangogh_integration.GogOpenCriticIdProperty:
			for _, line := range lines {
				idSlug, score := parseReceptionLines(line)
				if idSlug != "" {
					if id, slug, ok := strings.Cut(idSlug, "/"); ok {
						propertyValues[vangogh_integration.GogOpenCriticIdProperty] = []string{id}
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
	for part := range strings.SplitSeq(line, "|") {
		if after, ok := strings.CutPrefix(part, "name="); ok {
			name = after
			name = strings.TrimSuffix(name, "}}")
		}
		if after, ok := strings.CutPrefix(part, "build="); ok {
			build = after
			build = strings.TrimSuffix(build, "}}")
		}
	}
	return name, build
}
