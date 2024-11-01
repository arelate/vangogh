package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
)

func Product(id string, rdx kevlar.ReadableRedux, hasSections []string) compton.PageElement {

	title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)
	p.RegisterStyles(compton_styles.Styles, "product-labels.css")

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")

	showToc := compton.InputValue(p, input_types.Button, "Sections")
	pageStack.Append(compton.FICenter(p, appNavLinks, showToc))

	/* Product details sections shortcuts */

	productSectionsLinks := compton_fragments.ProductSectionsLinks(p, hasSections)
	pageStack.Append(productSectionsLinks)

	pageStack.Append(compton.Attach(p, showToc, productSectionsLinks))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(poster)
	}

	/* Product title */

	productTitle := compton.HeadingText(title, 1)
	productTitle.AddClass("product-title")

	/* Product labels */

	fmtLabels := compton_fragments.FormatLabels(id, rdx)
	productLabels := compton.Labels(p, fmtLabels...).FontSize(size.Small).RowGap(size.XSmall).ColumnGap(size.XSmall)
	pageStack.Append(compton.FICenter(p, productTitle, productLabels))

	/* Product details sections */

	for ii, section := range hasSections {

		sectionTitle := compton_data.SectionTitles[section]
		summaryHeading := compton.DSTitle(p, sectionTitle)
		detailsSummary := compton.DSLarge(p, summaryHeading, ii == 0).
			BackgroundColor(color.Highlight).
			ForegroundColor(color.Foreground).
			MarkerColor(color.Gray).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset)
		detailsSummary.SetId(sectionTitle)

		ifh := compton.IframeExpandHost(p, section, "/"+section+"?id="+id)
		detailsSummary.Append(ifh)

		pageStack.Append(detailsSummary)
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(),
		compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}
