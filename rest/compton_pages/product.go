package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/gaugin_elements/product_labels"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/details_summary"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/iframe_expand"
	"github.com/boggydigital/compton/elements/inputs"
	"github.com/boggydigital/compton/elements/labels"
	"github.com/boggydigital/compton/elements/popup"
	"github.com/boggydigital/kevlar"
)

func Product(id string, rdx kevlar.ReadableRedux, hasSections []string, extLinks map[string][]string) compton.Element {

	title, ok := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)
	p.AppendStyle(product_labels.StyleProductLabels)

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")

	showToc := inputs.InputValue(p, input_types.Button, "Sections")
	pageStack.Append(flex_items.Center(p, appNavLinks, showToc))

	/* Product details sections shortcuts */

	productSectionsLinks := compton_fragments.ProductSectionsLinks(p, hasSections)
	pageStack.Append(productSectionsLinks)

	pageStack.Append(popup.Attach(p, showToc, productSectionsLinks))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(poster)
	}

	/* Product title */

	productTitle := els.HeadingText(title, 1)
	productTitle.AddClass("product-title")

	/* Product labels */

	fmtLabels := product_labels.FormatLabels(id, rdx, compton_data.LabelProperties...)
	productLabels := labels.Labels(p, fmtLabels...).FontSize(size.Small).RowGap(size.XSmall).ColumnGap(size.XSmall)
	pageStack.Append(flex_items.Center(p, productTitle, productLabels))

	/* Product details sections */

	for _, section := range hasSections {

		sectionTitle := compton_data.SectionTitles[section]
		summaryHeading := compton_fragments.DetailsSummaryTitle(p, sectionTitle)
		detailsSummary := details_summary.
			Larger(p, summaryHeading, section == compton_data.PropertiesSection).
			BackgroundColor(color.Highlight).
			ForegroundColor(color.Foreground).
			MarkerColor(color.Gray).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset)
		detailsSummary.SetId(sectionTitle)

		switch section {
		case compton_data.PropertiesSection:
			if productProperties := compton_fragments.ProductProperties(p, id, rdx); productProperties != nil {
				detailsSummary.Append(productProperties)
			}
		case compton_data.ExternalLinksSection:
			if externalLinks := compton_fragments.ProductExternalLinks(p, extLinks); externalLinks != nil {
				detailsSummary.Append(externalLinks)
			}
		default:
			ifh := iframe_expand.IframeExpandHost(p, section, "/"+section+"?id="+id)
			detailsSummary.Append(ifh)
		}
		pageStack.Append(detailsSummary)
	}

	/* Standard app footer */

	pageStack.Append(els.Br(), compton_fragments.Footer(p))

	return p
}
