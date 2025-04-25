package compton_pages

import (
	_ "embed"
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
	"slices"
	"strings"
	"time"
)

var (
	propertiesSections = map[string]string{
		vangogh_integration.ChangelogProperty:   compton_data.ChangelogSection,
		vangogh_integration.ScreenshotsProperty: compton_data.ScreenshotsSection,
		vangogh_integration.VideoIdProperty:     compton_data.VideosSection,
	}
	propertiesSectionsOrder = []string{
		vangogh_integration.ChangelogProperty,
		vangogh_integration.ScreenshotsProperty,
		vangogh_integration.VideoIdProperty,
	}

	dataTypesSections = map[vangogh_integration.ProductType]string{
		vangogh_integration.SteamAppNews:                 compton_data.SteamNewsSection,
		vangogh_integration.SteamDeckCompatibilityReport: compton_data.SteamDeckSection,
	}

	dataTypesSectionsOrder = []vangogh_integration.ProductType{
		vangogh_integration.SteamAppNews,
		vangogh_integration.SteamDeckCompatibilityReport,
	}
)

func Product(id string, rdx redux.Readable) compton.PageElement {

	title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)
	p.RegisterStyles(compton_styles.Styles, "product.css")

	// tinting document background color to the representative product color
	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if repColor, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && repColor != issa.NeutralRepColor {
			p.SetAttribute("style", "--c-rep:"+repColor)
		}
	}

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")

	showTocNavLinks, showTocLink := compton_fragments.ShowToc(p)

	/* Determine which sections should the product page have */

	hasSections := make([]string, 0)

	hasSections = append(hasSections, compton_data.InformationSection)

	if rdx.HasKey(vangogh_integration.DescriptionOverviewProperty, id) {
		hasSections = append(hasSections, compton_data.DescriptionSection)
	}

	hasSections = append(hasSections, compton_data.ReceptionSection)

	offeringsCount := 0
	for _, rpp := range compton_data.OfferingsProperties {
		var rps []string
		if rps, ok = rdx.GetAllValues(rpp, id); ok {
			offeringsCount += len(rps)
		}
	}

	if offeringsCount > 0 {
		hasSections = append(hasSections, compton_data.OfferingsSection)
	}

	hasSections = append(hasSections, compton_data.LinksSection)

	for _, property := range propertiesSectionsOrder {
		if section, ok := propertiesSections[property]; ok {
			if val, sure := rdx.GetLastVal(property, id); sure && val != "" {
				hasSections = append(hasSections, section)
			}
		}
	}

	for _, dt := range dataTypesSectionsOrder {
		if section, ok := dataTypesSections[dt]; ok {
			if rdx.HasValue(vangogh_integration.TypesProperty, id, dt.String()) {
				hasSections = append(hasSections, section)
			}
		}
	}

	if val, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && val == vangogh_integration.TrueValue {
		hasSections = append(hasSections, compton_data.InstallersSection)
	}

	/* Product details sections shortcuts */

	productSectionsLinks := compton.SectionsLinks(p, hasSections, compton_data.SectionTitles)
	pageStack.Append(compton.Attach(p, showTocLink, productSectionsLinks))

	topLevelNav := []compton.Element{appNavLinks, showTocNavLinks, productSectionsLinks}

	pageStack.Append(compton.FICenter(p, topLevelNav...))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(compton.FICenter(p, poster))
	}

	/* Product title */

	productTitle := compton.Heading(2)
	productTitle.Append(compton.Fspan(p, title).TextAlign(align.Center))
	productTitle.AddClass("product-title")

	/* Product labels */

	pageStack.Append(compton.FICenter(p, productTitle))

	/* Product summary properties */

	properties, values := compton_fragments.SummarizeProductProperties(id, rdx)
	osSymbols := make([]compton.Symbol, 0, 2)

	summaryRow := compton.Frow(p).FontSize(size.XSmall)

	for _, property := range properties {
		switch property {
		case vangogh_integration.OperatingSystemsProperty:
			osValues := vangogh_integration.ParseManyOperatingSystems(values[property])
			for _, os := range compton_data.OSOrder {
				if slices.Contains(osValues, os) {
					osSymbols = append(osSymbols, compton_data.OperatingSystemSymbols[os])
				}
			}
			summaryRow.PropIcons(compton_data.PropertyTitles[property], osSymbols...)
		default:
			summaryRow.PropVal(compton_data.PropertyTitles[property], strings.Join(values[property], ", "))
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
			DetailsMarginBlockEnd(size.Unset).
			SummaryMarginBlockEnd(size.Normal)
		detailsSummary.SetId(sectionTitle)
		detailsSummary.SetTabIndex(ii + 1)

		switch section {
		case compton_data.InformationSection:
			productBadges := compton.FlexItems(p, direction.Row).ColumnGap(size.XSmall)
			for _, fmtBadge := range compton_fragments.FormatBadges(id, rdx) {
				badge := compton.Badge(p, fmtBadge.Title, fmtBadge.Color, color.Highlight)
				badge.AddClass(fmtBadge.Class)
				productBadges.Append(badge)
			}
			detailsSummary.AppendBadges(productBadges)
		case compton_data.SteamDeckSection:
			if sdccp, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {

				var deckCompatColor color.Color
				switch sdccp {
				case "Verified":
					deckCompatColor = color.Green
				case "Playable":
					deckCompatColor = color.Orange
				case "Unsupported":
					deckCompatColor = color.Red
				default:
					deckCompatColor = color.Gray
				}

				steamDeckBadge := compton.Badge(p, sdccp, deckCompatColor, color.Highlight)
				detailsSummary.AppendBadges(steamDeckBadge)
			}
		case compton_data.ReceptionSection:
			receptionBadges := compton.FlexItems(p, direction.Row).ColumnGap(size.XSmall)

			if tp, sure := rdx.GetLastVal(vangogh_integration.TopPercentProperty, id); sure && tp != "" {
				topPercentBadge := compton.Badge(p, fmt.Sprintf("Top %s", tp), color.Green, color.Highlight)
				receptionBadges.Append(topPercentBadge)
			}

			if srep, ok := rdx.GetLastVal(vangogh_integration.SummaryReviewsProperty, id); ok {

				var receptionColor color.Color
				switch srep {
				case vangogh_integration.RatingPositive:
					receptionColor = color.Green
				case vangogh_integration.RatingNegative:
					receptionColor = color.Red
				case vangogh_integration.RatingMixed:
					receptionColor = color.Yellow
				default:
					receptionColor = color.Gray
				}

				ratingsReviews := srep

				if srap, sure := rdx.GetLastVal(vangogh_integration.SummaryRatingProperty, id); sure {
					ratingsReviews = compton_fragments.FmtAggregatedRating(srap)
				}

				receptionBadges.Append(compton.Badge(p, ratingsReviews, receptionColor, color.Highlight))

			}

			detailsSummary.AppendBadges(receptionBadges)
		case compton_data.SteamNewsSection:
			if lcus, ok := rdx.GetLastVal(vangogh_integration.SteamLastCommunityUpdateProperty, id); ok && lcus != "" {
				if lcut, err := time.Parse(time.RFC3339, lcus); err == nil {

					updateColor := color.Gray
					if lcut.After(time.Now().Add(-1 * time.Hour * 24 * 30)) {
						updateColor = color.Green
					}

					lastUpdateBadge := compton.Badge(p, lcut.Format("Jan 2, '06"), updateColor, color.Highlight)
					detailsSummary.AppendBadges(lastUpdateBadge)
				}
			}
		case compton_data.InstallersSection:
			if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
				pvr := vangogh_integration.ParseValidationResult(pvrs)

				validationBadge := compton.Badge(p,
					pvr.HumanReadableString(),
					compton_fragments.ValidationResultsColors[pvr],
					color.Highlight)
				detailsSummary.AppendBadges(validationBadge)
			}
		default:
			detailsSummary.SummaryMarginBlockEnd(size.Normal)
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
