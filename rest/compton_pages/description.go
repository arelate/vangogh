package compton_pages

import (
	"net/url"
	"path"
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func Description(id string, pageTitle string, descOverview, descFeatures string, rdx redux.Readable) compton.PageElement {

	p := compton.Page(pageTitle)

	p.RegisterStyles(compton_styles.Styles, "description.css")

	pageStack := compton.FlexItems(p, direction.Column).
		RowGap(size.Normal)
	p.Append(compton.FICenter(p, pageStack))

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	headingRow := compton.FlexItems(p, direction.Column).RowGap(size.XSmall)

	heading := compton.Heading(2)
	heading.Append(compton.Fspan(p, pageTitle).TextAlign(align.Center))
	//heading.SetAttribute("style", "view-transition-name:product-title-"+id)
	headingRow.Append(heading)

	subHeading := compton.Heading(3)
	subHeading.Append(compton.Fspan(p, "Description").
		TextAlign(align.Center).
		ForegroundColor(color.Gray))
	headingRow.Append(subHeading)

	pageStack.Append(compton.FICenter(p, headingRow))

	descriptionStack := compton.FlexItems(p, direction.Column).
		AlignItems(align.Start).
		MaxWidth(size.MaxWidth)
	pageStack.Append(descriptionStack)

	descriptionDiv := compton.Div()
	descriptionDiv.AddClass("description")
	descriptionStack.Append(descriptionDiv)

	if descOverview == "" {
		fs := compton.Fspan(p, "Description is not available for this product").
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		descriptionDiv.Append(compton.FICenter(p, fs))
	} else {
		descOverview = rewriteDescriptionImagesLinks(descOverview)
		descOverview = rewriteGameLinks(descOverview)
		descOverview = rewriteLinksAsTargetTop(descOverview)
		descOverview = fixQuotes(descOverview)
		descOverview = replaceDataFallbackUrls(descOverview)
		descOverview = rewriteVideoAsInline(descOverview)

		descriptionDiv.Append(compton.Text(descOverview))
	}

	featuresDiv := compton.Div()
	featuresDiv.AddClass("description__features")
	if descFeatures != "" {
		featuresDiv.Append(compton.Text(implicitToExplicitList(descFeatures)))
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

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
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
