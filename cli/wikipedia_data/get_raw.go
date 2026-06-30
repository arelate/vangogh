package wikipedia_data

import (
	"errors"
	"iter"
	"maps"
	"slices"
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

	return ReduceRaw(maps.Keys(wikipediaGogIds), kvRaw)
}

func ReduceRaw(wikipediaIds iter.Seq[string], kvRaw kevlar.KeyValues) error {

	dataType := vangogh_integration.WikipediaRaw

	rra := nod.NewProgress(" reducing %s...", dataType)
	defer rra.Done()

	rdx, err := redux.NewWriter(vangogh_integration.AbsReduxDir(), vangogh_integration.WikipediaRawProperties()...)
	if err != nil {
		return err
	}

	rawReductions := shared_data.InitReductions(vangogh_integration.WikipediaRawProperties()...)

	for wikipediaId := range wikipediaIds {
		if !kvRaw.Has(wikipediaId) {
			nod.LogError(errors.New("missing: " + dataType.String() + ", " + wikipediaId))
			continue
		}

		if err = reduceRawProduct(wikipediaId, kvRaw, rawReductions, rdx); err != nil {
			return err
		}

		rra.Increment()
	}

	return shared_data.WriteReductions(rdx, rawReductions)
}

func reduceRawProduct(wikipediaId string, kvRaw kevlar.KeyValues, piv shared_data.PropertyIdValues, rdx redux.Readable) error {

	rcRaw, err := kvRaw.Get(wikipediaId)
	if err != nil {
		return err
	}
	defer rcRaw.Close()

	infoboxLines, err := wikipedia_integration.FilterInfoboxLines(rcRaw)
	if err != nil {
		return err
	}

	rawCredits := reduceRawCredits(infoboxLines)

	parsedCredits := parseCredits(rawCredits)

	for property := range piv {

		if values, ok := parsedCredits[property]; ok && shared_data.IsNotEmpty(values...) {
			piv[property][wikipediaId] = values
		}

	}

	return nil
}

var prefixedProperties = map[string]string{
	"|creator":     vangogh_integration.WikipediaCreatorsProperty,
	"| creator":    vangogh_integration.WikipediaCreatorsProperty,
	"|director":    vangogh_integration.WikipediaDirectorsProperty,
	"| director":   vangogh_integration.WikipediaDirectorsProperty,
	"|producer":    vangogh_integration.WikipediaProducersProperty,
	"| producer":   vangogh_integration.WikipediaProducersProperty,
	"|designer":    vangogh_integration.WikipediaDesignersProperty,
	"| designer":   vangogh_integration.WikipediaDesignersProperty,
	"|programmer":  vangogh_integration.WikipediaProgrammersProperty,
	"| programmer": vangogh_integration.WikipediaProgrammersProperty,
	"|artist":      vangogh_integration.WikipediaArtistsProperty,
	"| artist":     vangogh_integration.WikipediaArtistsProperty,
	"|writer":      vangogh_integration.WikipediaWritersProperty,
	"| writer":     vangogh_integration.WikipediaWritersProperty,
	"|composer":    vangogh_integration.WikipediaComposersProperty,
	"| composer":   vangogh_integration.WikipediaComposersProperty,
}

var listPrefixes = []string{
	"{{Unbulleted list",
	"{{unbulleted list",
	"{{Plainlist",
	"{{plainlist",
	"{{Ubl",
	"{{ubl",
}

func reduceRawCredits(infoboxLines []string) map[string][]string {

	rawCredits := make(map[string][]string)

	list := false
	lastProperty := ""

	for _, line := range infoboxLines {

		line = strings.TrimSpace(line)

		if strings.HasPrefix(line, wikipedia_integration.ListSfx) {
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
				list = false
				break
			}
		}

		for _, lp := range listPrefixes {
			if lastProperty != "" && strings.Contains(line, lp) && !strings.Contains(line, wikipedia_integration.ListSfx) {
				list = true
			}
		}

		if !list {
			lastProperty = ""
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
			for _, li := range listItems {
				if slices.Contains(parsedValues, li) {
					continue
				}
				parsedValues = append(parsedValues, li)
			}
			continue
		}
	}

	return parsedValues
}

func parseList(value string) []string {

	value = strings.TrimSpace(value)
	if value == "" {
		return nil
	}

	for _, lp := range listPrefixes {
		value = strings.TrimPrefix(value, lp)
	}

	value = replaceListSeparators(value)

	for range 10 {
		value = trimEnclosed(value, "<ref", "/ref>")
	}

	value = strings.Replace(value, "<ref name=\"bombinfo\"/>", "", -1)
	value = strings.Replace(value, "<ref name=FAQ/>", "", -1)
	value = strings.Replace(value, "<ref name=\"DM3Manual\"/>", "", -1)
	value = strings.Replace(value, "[ lead ]", "", -1)
	//value = trimEnclosed(value, "<", "/>")

	value = trimEnclosed(value, "{{efn", "}}")
	value = trimEnclosed(value, "{{Sfn", "}}")

	value = trimEnclosed(value, "(", ")")

	value = strings.TrimSuffix(value, wikipedia_integration.ListSfx)

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

	value = strings.Trim(value, "[]'|* ")
	value = strings.TrimSpace(value)

	return value
}

var alternativeListSeparators = []string{
	"<br>",
	"<br >",
	"<br/>",
	"<br />",
	", ",
}

func replaceListSeparators(value string) string {
	for _, als := range alternativeListSeparators {
		value = strings.Replace(value, als, "|", -1)
	}
	return value
}

func trimEnclosed(s string, open, close string) string {

	for strings.Contains(s, open) && strings.Contains(s, close) {
		openIndex := strings.Index(s, open)
		closeIndex := strings.Index(s, close)

		if closeIndex > openIndex {
			s = strings.Replace(s, s[openIndex:closeIndex+len(close)], "", 1)
		} else {
			break
		}
	}

	return s
}
