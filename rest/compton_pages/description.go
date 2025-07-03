package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
	"net/url"
	"path"
	"strings"
)

func Description(id string, rdx redux.Readable) compton.PageElement {
	desc := ""
	if dop, ok := rdx.GetLastVal(vangogh_integration.DescriptionOverviewProperty, id); ok {
		desc = dop
	}

	s := compton_fragments.ProductSection(compton_data.DescriptionSection, id, rdx)

	descriptionDiv := compton.Div()
	descriptionDiv.AddClass("description")
	s.Append(descriptionDiv)

	if desc == "" {
		fs := compton.Fspan(s, "Description is not available for this product").
			ForegroundColor(color.RepGray).
			TextAlign(align.Center)
		descriptionDiv.Append(compton.FICenter(s, fs))
	} else {
		desc = rewriteDescriptionImagesLinks(desc)
		desc = rewriteGameLinks(desc)
		desc = rewriteLinksAsTargetTop(desc)
		desc = fixQuotes(desc)
		desc = replaceDataFallbackUrls(desc)
		desc = rewriteVideoAsInline(desc)

		descriptionDiv.Append(compton.Text(desc))
	}

	featuresDiv := compton.Div()
	featuresDiv.AddClass("description__features")
	if dfp, ok := rdx.GetLastVal(vangogh_integration.DescriptionFeaturesProperty, id); ok {
		featuresDiv.Append(compton.Text(implicitToExplicitList(dfp)))
	}

	descriptionDiv.Append(featuresDiv)

	copyrightsDiv := compton.Div()
	copyrightsDiv.AddClass("description__copyrights")
	descriptionDiv.Append(copyrightsDiv)

	copyright := ""
	if cp, ok := rdx.GetLastVal(vangogh_integration.CopyrightsProperty, id); ok {
		copyright = cp
	}
	if copyright != "" {
		cd := compton.DivText(copyright)
		copyrightsDiv.Append(cd)
	}

	addtReqs := ""
	if arp, ok := rdx.GetLastVal(vangogh_integration.AdditionalRequirementsProperty, id); ok {
		addtReqs = arp
	}
	if addtReqs != "" {
		ard := compton.DivText(addtReqs)
		copyrightsDiv.Append(ard)
	}

	return s
}

func rewriteDescriptionImagesLinks(desc string) string {

	itemsUrls := vangogh_integration.ExtractDescItems(desc)

	for _, itemUrl := range itemsUrls {
		if u, err := url.Parse(itemUrl); err != nil {
			continue
		} else {
			ggUrl := "/description-images" + u.Path
			desc = strings.Replace(desc, itemUrl, ggUrl, -1)
		}
	}

	return desc
}

func rewriteGameLinks(desc string) string {
	gameLinks := vangogh_integration.ExtractGameLinks(desc)

	for _, gameLink := range gameLinks {
		if u, err := url.Parse(gameLink); err != nil {
			continue
		} else {
			_, slug := path.Split(u.Path)
			ggUrl := "/product?slug=" + slug
			desc = strings.Replace(desc, gameLink, ggUrl, -1)
		}
	}

	return desc
}

func rewriteLinksAsTargetTop(desc string) string {
	return strings.Replace(desc, "<a ", "<a target='_top' ", -1)
}

func rewriteVideoAsInline(desc string) string {
	return strings.Replace(desc, "<video ", "<video playsinline ", -1)
}

func fixQuotes(desc string) string {
	return strings.Replace(desc, "”", "\"", -1)
}

func replaceDataFallbackUrls(desc string) string {
	return strings.Replace(desc, "data-fallbackurl", "poster", -1)
}

const doubleNewLineChar = "\n\n"
const newLineChar = "\n"
const emDashCode = "\u2013"

// implicitToExplicitList looks for embedded characters
// that GOG.com is using for <ul> lists creation, e.g.
// https://www.gog.com/en/game/deaths_gambit
// and replaces that segment with explicit unordered lists.
// Currently known characters are listed as consts above this func.
func implicitToExplicitList(text string) string {
	var items []string
	if strings.Contains(text, doubleNewLineChar) {
		items = strings.Split(text, doubleNewLineChar)
	} else if strings.Contains(text, newLineChar) {
		items = strings.Split(text, newLineChar)
	} else if strings.Contains(text, emDashCode) {
		items = strings.Split(text, emDashCode)
	}

	if len(items) > 0 {
		builder := strings.Builder{}
		builder.WriteString("<ul>")
		for _, item := range items {
			builder.WriteString("<li>" + item + "</li>")
		}
		builder.WriteString("</ul>")
		text = builder.String()
	}

	return text
}
