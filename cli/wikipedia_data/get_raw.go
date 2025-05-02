package wikipedia_data

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

func GetRaw(wikipediaGogIds map[string][]string, force bool) error {

	gra := nod.NewProgress("getting %s...", vangogh_integration.WikipediaRaw)
	defer gra.Done()

	rawDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.WikipediaRaw)
	if err != nil {
		return err
	}

	kvRaw, err := kevlar.New(rawDir, kevlar.TxtExt)
	if err != nil {
		return err
	}

	gra.TotalInt(len(wikipediaGogIds))

	if err = fetch.Items(maps.Keys(wikipediaGogIds), reqs.WikipediaRaw(), kvRaw, gra, force); err != nil {
		return err
	}

	return ReduceRaw(wikipediaGogIds, kvRaw)
}

func ReduceRaw(wikipediaGogIds map[string][]string, kvRaw kevlar.KeyValues) error {

	dataType := vangogh_integration.WikipediaRaw

	rra := nod.NewProgress(" reducing %s...", dataType)
	defer rra.Done()

	reduxDir, err := pathways.GetAbsRelDir(vangogh_integration.Redux)
	if err != nil {
		return err
	}

	rdx, err := redux.NewWriter(reduxDir, vangogh_integration.WikipediaRawProperties()...)
	if err != nil {
		return err
	}

	rawReductions := shared_data.InitReductions(vangogh_integration.WikipediaRawProperties()...)

	rra.TotalInt(len(wikipediaGogIds))

	for wikipediaId, gogIds := range wikipediaGogIds {
		if !kvRaw.Has(wikipediaId) {
			nod.LogError(fmt.Errorf("%s is missing %s", dataType, wikipediaId))
			continue
		}

		if err = reduceRawProduct(gogIds, wikipediaId, kvRaw, rawReductions, rdx); err != nil {
			return err
		}

		rra.Increment()
	}

	return shared_data.WriteReductions(rdx, rawReductions)
}

func reduceRawProduct(gogIds []string, wikipediaId string, kvRaw kevlar.KeyValues, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

	rcRaw, err := kvRaw.Get(wikipediaId)
	if err != nil {
		return err
	}
	defer rcRaw.Close()

	infoboxLines, err := filterInfoboxLines(rcRaw)
	if err != nil {
		return err
	}

	rawCredits := reduceRawCredits(infoboxLines)

	parsedCredits := parseCredits(rawCredits)

	for property := range piv {

		for _, gogId := range gogIds {
			if values, ok := parsedCredits[property]; ok && shared_data.IsNotEmpty(values...) {
				piv[property][gogId] = values
			}
		}

	}

	return nil
}

const (
	infoboxVideoGamePfx = "{{Infobox video game"
	infoboxVgPfx        = "{{Infobox VG"
	listPfx             = "{{"
	listSfx             = "}}"
)

func filterInfoboxLines(r io.Reader) ([]string, error) {

	infoboxLines := make([]string, 0)

	ts := bufio.NewScanner(r)

	scanning := false
	previousLine := ""
	listCounter := 0

	for ts.Scan() {
		line := ts.Text()
		line = strings.TrimSpace(line)

		listCounter += strings.Count(line, listPfx)
		listCounter -= strings.Count(line, listSfx)

		if !scanning &&
			(strings.Contains(line, infoboxVideoGamePfx) ||
				strings.Contains(line, infoboxVgPfx)) {
			scanning = true
			continue
		}
		if listCounter == 0 && strings.HasPrefix(line, listSfx) {
			break
		}
		if scanning && line == "" && strings.HasSuffix(previousLine, listSfx) {
			break
		}

		if scanning {
			infoboxLines = append(infoboxLines, line)
		}

		previousLine = line
	}

	if err := ts.Err(); err != nil {
		return nil, err
	}

	return infoboxLines, nil
}

var prefixedProperties = map[string]string{
	"|creator":     vangogh_integration.CreatorsProperty,
	"| creator":    vangogh_integration.CreatorsProperty,
	"|director":    vangogh_integration.DirectorsProperty,
	"| director":   vangogh_integration.DirectorsProperty,
	"|producer":    vangogh_integration.ProducersProperty,
	"| producer":   vangogh_integration.ProducersProperty,
	"|designer":    vangogh_integration.DesignersProperty,
	"| designer":   vangogh_integration.DesignersProperty,
	"|programmer":  vangogh_integration.ProgrammersProperty,
	"| programmer": vangogh_integration.ProgrammersProperty,
	"|artist":      vangogh_integration.ArtistsProperty,
	"| artist":     vangogh_integration.ArtistsProperty,
	"|writer":      vangogh_integration.WritersProperty,
	"| writer":     vangogh_integration.WritersProperty,
	"|composer":    vangogh_integration.ComposersProperty,
	"| composer":   vangogh_integration.ComposersProperty,
}

var listPrefixes = []string{
	"{{Unbulleted list",
	"{{plainlist",
	"{{ubl",
}

func reduceRawCredits(infoboxLines []string) map[string][]string {

	rawCredits := make(map[string][]string)

	list := false
	lastProperty := ""

	for _, line := range infoboxLines {

		line = strings.TrimSpace(line)

		if strings.HasPrefix(line, listSfx) {
			list = false
			lastProperty = ""
		}

		if list && lastProperty != "" {
			rawCredits[lastProperty] = append(rawCredits[lastProperty], line)
		}

		for pfx, property := range prefixedProperties {
			if strings.HasPrefix(line, pfx) {
				rawCredits[property] = append(rawCredits[property], line)
				lastProperty = property
				break
			}
		}

		for _, lp := range listPrefixes {
			if strings.Contains(line, lp) && !strings.Contains(line, listSfx) {
				list = true
			}
		}
	}

	return rawCredits
}

func parseCredits(rawCredits map[string][]string) map[string][]string {

	parsedCredits := make(map[string][]string)

	for property, rawValues := range rawCredits {
		parsedValues := parseCreditValues(property, rawValues...)

		if shared_data.IsNotEmpty(parsedValues...) {
			parsedCredits[property] = parsedValues
		}
	}

	return parsedCredits
}

func parseCreditValues(property string, rawValues ...string) []string {

	parsedValues := make([]string, 0, len(rawValues))

	for _, rv := range rawValues {

		for pfx, pp := range prefixedProperties {
			if pp == property {
				rv = strings.TrimPrefix(rv, pfx)
			}
		}

		if _, sp, ok := strings.Cut(rv, "="); ok {
			rv = sp
		}

		if listItems := parseList(rv); len(listItems) > 0 {
			parsedValues = append(parsedValues, listItems...)
			continue
		}
	}

	return parsedValues
}

func parseList(value string) []string {
	value = strings.TrimSpace(value)

	for _, lp := range listPrefixes {
		value = strings.TrimPrefix(value, lp)
	}

	value = replaceListSeparators(value)

	value = trimEnclosed(value, "{{efn", "}}")
	//value = trimEnclosed(value, "<", "/>")

	value = strings.TrimSuffix(value, listSfx)

	if parts := strings.Split(value, "|"); len(parts) > 1 {
		return parseListItems(parts...)
	} else if tv := trimWikiText(value); tv != "" {
		return []string{tv}
	} else {
		return nil
	}
}

func parseListItems(parts ...string) []string {
	listItems := make([]string, 0, len(parts))
	for _, part := range parts {

		part = trimWikiText(part)

		if part != "" {
			listItems = append(listItems, part)
		}
	}

	return listItems
}

const (
	commentPfx = "<!--"
	commentSfx = "-->"
)

func trimWikiText(value string) string {

	value = strings.TrimSpace(value)

	if strings.HasPrefix(value, commentPfx) &&
		strings.HasSuffix(value, commentSfx) {
		return ""
	}

	value = strings.Trim(value, "[]'|*")
	value = strings.TrimSpace(value)

	return value
}

var alternativeListSeparators = []string{
	"<br>",
	"<br >",
	"<br/>",
	"<br />",
}

func replaceListSeparators(value string) string {
	for _, als := range alternativeListSeparators {
		value = strings.Replace(value, als, "|", -1)
	}
	return value
}

func trimEnclosed(s string, open, close string) string {

	for strings.Contains(s, open) && strings.Contains(s, close) {
		firstIndex := strings.Index(s, open)
		lastIndex := strings.Index(s, close)

		s = strings.Replace(s, s[firstIndex:lastIndex+len(close)], "", 1)
	}

	return s
}
