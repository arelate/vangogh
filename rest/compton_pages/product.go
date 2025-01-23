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
	"github.com/boggydigital/kevlar"
	"golang.org/x/exp/slices"
	"strings"
)

const colorBlendClass = "color-blend"

const (
	theoInstallTemplate       = "theo install {id}"
	theoUninstallTemplate     = "theo uninstall {id} -force"
	theoWineInstallTemplate   = "theo wine-install {id}"
	theoWineUninstallTemplate = "theo wine-uninstall {id} -force"
)

func Product(id string, rdx kevlar.ReadableRedux, hasSections []string) compton.PageElement {

	title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)
	p.RegisterStyles(compton_styles.Styles, "product.css")

	// tinting document background color to the representative product color
	if repColor, ok := rdx.GetLastVal(vangogh_integration.RepImageColorProperty, id); ok && repColor != issa.NeutralRepColor {
		compton.SetTint(p, repColor)
	}

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	appNavLinks.AddClass(colorBlendClass)

	showToc := compton.InputValue(p, input_types.Button, compton.SectionLinksTitle)
	showToc.AddClass(colorBlendClass)

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
	productTitle.AddClass(colorBlendClass)
	productTitle.Append(compton.Fspan(p, title).TextAlign(align.Center))
	productTitle.AddClass("product-title")

	/* Product labels */

	fmtLabels := compton_fragments.FormatLabels(id, rdx)
	productLabels := compton.Labels(p, fmtLabels...).FontSize(size.Small).RowGap(size.XSmall).ColumnGap(size.XSmall)
	pageStack.Append(compton.FICenter(p, productTitle, productLabels))

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

	for _, section := range hasSections {

		sectionTitle := compton_data.SectionTitles[section]
		summaryHeading := compton.DSTitle(p, sectionTitle)
		detailsSummary := compton.DSLarge(p, summaryHeading, false).
			BackgroundColor(color.Highlight).
			ForegroundColor(color.Foreground).
			MarkerColor(color.Gray).
			SummaryMarginBlockEnd(size.Normal).
			DetailsMarginBlockEnd(size.Unset)
		detailsSummary.SetId(sectionTitle)
		detailsSummary.AddClassSummary(colorBlendClass)

		switch section {
		case compton_data.SteamDeckSection:
			if sdc := compton_fragments.SteamDeckCompatibility(p, id, rdx); sdc != nil {
				detailsSummary.AppendSummary(sdc)
			}
		case compton_data.SteamReviewsSection:
			if srsd := compton_fragments.SteamReviewScoreDesc(p, id, rdx); srsd != nil {
				detailsSummary.AppendSummary(srsd)
			}
		}

		ifh := compton.IframeExpandHost(p, section, "/"+section+"?id="+id)
		detailsSummary.Append(ifh)

		pageStack.Append(detailsSummary)
	}

	/* Theo commands */

	if owned, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && owned == vangogh_integration.TrueValue {
		var os []vangogh_integration.OperatingSystem
		if vals, ok := rdx.GetAllValues(vangogh_integration.OperatingSystemsProperty, id); ok {
			os = vangogh_integration.ParseManyOperatingSystems(vals)
		}

		if slices.Contains(os, vangogh_integration.MacOS) {
			pageStack.Append(theoCommand(p, theoInstallTemplate, id))
			pageStack.Append(theoCommand(p, theoUninstallTemplate, id))
		} else {
			pageStack.Append(theoCommand(p, theoWineInstallTemplate, id))
			pageStack.Append(theoCommand(p, theoWineUninstallTemplate, id))
		}
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
