package compton_pages

import (
	_ "embed"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
	"slices"
	"strings"
)

func Product(id string, rdx redux.Readable, hasSections []string) compton.PageElement {

	title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)
	p.RegisterStyles(compton_styles.Styles, "product.css")

	// tinting document background color to the representative product color
	if repColor, ok := rdx.GetLastVal(vangogh_integration.RepImageColorProperty, id); ok && repColor != issa.NeutralRepColor {
		p.SetAttribute("style", "--c-rep:"+repColor)
	}

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")

	showToc := compton.InputValue(p, input_types.Button, compton.SectionLinksTitle)

	/* Product details sections shortcuts */

	productSectionsLinks := compton.SectionsLinks(p, hasSections, compton_data.SectionTitles)
	pageStack.Append(compton.Attach(p, showToc, productSectionsLinks))

	topLevelNav := []compton.Element{appNavLinks, showToc, productSectionsLinks}

	pageStack.Append(compton.FICenter(p, topLevelNav...))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(compton.FICenter(p, poster))
	}

	/* Product title */

	productTitle := compton.Heading(1)
	productTitle.Append(compton.Fspan(p, title).TextAlign(align.Center))
	productTitle.AddClass("product-title")

	/* Product labels */

	fmtLabels := compton_fragments.FormatLabels(id, rdx)
	productLabels := compton.Labels(p, fmtLabels...).FontSize(size.Small).RowGap(size.XSmall).ColumnGap(size.XSmall)
	pageStack.Append(compton.FICenter(p, productTitle, productLabels))

	/* Product short description */

	if shortDesc, yes := rdx.GetLastVal(vangogh_integration.ShortDescriptionProperty, id); yes {
		oneLiner := compton.OneLiner(p)

		shortDescFspan := compton.Fspan(p, shortDesc).
			ForegroundColor(color.Gray).
			FontSize(size.Small).
			TextAlign(align.Center)
		oneLiner.Append(shortDescFspan)
		oneLiner.AddClass("short-description")

		pageStack.Append(compton.FICenter(p, oneLiner))
	}

	/* Product summary properties */

	properties, values := compton_fragments.SummarizeProductProperties(id, rdx)
	osSymbols := make([]compton.Symbol, 0, 2)

	summaryRow := compton.Frow(p).
		FontSize(size.Small)

	for _, p := range properties {
		switch p {
		case vangogh_integration.OperatingSystemsProperty:
			osValues := vangogh_integration.ParseManyOperatingSystems(values[p])
			for _, os := range compton_data.OSOrder {
				if slices.Contains(osValues, os) {
					osSymbols = append(osSymbols, compton_data.OperatingSystemSymbols[os])
				}
			}
			summaryRow.PropIcons(compton_data.PropertyTitles[p], osSymbols...)
		default:
			summaryRow.PropVal(compton_data.PropertyTitles[p], strings.Join(values[p], ", "))
		}
	}
	pageStack.Append(compton.FICenter(p, summaryRow))

	/* Product details sections */

	for ii, section := range hasSections {

		sectionTitle := compton_data.SectionTitles[section]
		detailsSummary := compton.DSLarge(p, sectionTitle, false).
			BackgroundColor(color.Highlight).
			ForegroundColor(color.Foreground).
			MarkerColor(color.Gray).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset)
		detailsSummary.SetId(sectionTitle)
		detailsSummary.SetTabIndex(ii + 1)

		switch section {
		case compton_data.SteamDeckSection:
			if sdct, sdcc := compton_fragments.SteamDeckCompatibility(id, rdx); sdct != "" {
				detailsSummary.SetLabelText(sdct)
				detailsSummary.SetLabelBackgroundColor(sdcc)
				detailsSummary.SetLabelForegroundColor(color.Highlight)
			}
		case compton_data.SteamReviewsSection:
			if srsdt, srsdc := compton_fragments.SteamReviewScoreDesc(id, rdx); srsdt != "" {
				detailsSummary.SetLabelText(srsdt)
				detailsSummary.SetLabelBackgroundColor(srsdc)
				detailsSummary.SetLabelForegroundColor(color.Highlight)
			}
		case compton_data.InstallersSection:
			if pvrt, pvrc := compton_fragments.ProductValidationResult(id, rdx); pvrt != "" {
				detailsSummary.SetLabelText(pvrt)
				detailsSummary.SetLabelBackgroundColor(pvrc)
				detailsSummary.SetLabelForegroundColor(color.Highlight)
			}
		}

		ifh := compton.IframeExpandHost(p, section, "/"+section+"?id="+id)
		detailsSummary.Append(ifh)

		pageStack.Append(detailsSummary)
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(),
		compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}

func theoCommand(r compton.Registrar, cmdTemplate, id string) compton.Element {
	cmd := strings.Replace(cmdTemplate, "{id}", id, -1)
	cmdContainer := compton.Fspan(r, cmd).
		ForegroundColor(color.Gray).
		TextAlign(align.Center)
	cmdContainer.AddClass("cmd")
	return compton.FICenter(r, cmdContainer)
}
