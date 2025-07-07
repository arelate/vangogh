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
	"github.com/boggydigital/compton/consts/loading"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
	"slices"
	"strings"
	"time"
)

var openSections = []string{
	compton_data.InfoSection,
}

func Product(id string, rdx redux.Readable) compton.PageElement {

	title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)

	p.AppendSpeculationRules("/*")

	p.RegisterStyles(compton_styles.Styles, "product.css", "gpp-badge.css")

	// tinting document background color to the representative product color
	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if repColor, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && repColor != issa.NeutralRepColor {
			p.SetAttribute("style", "--c-rep:"+repColor)
		}
	}

	/* App navigation */

	menuNavLink := compton_fragments.MenuNav(p, title, id, rdx)
	pageStack.Append(menuNavLink)

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(compton.FICenter(p, poster))
	}

	/* Product title */

	productTitle := compton.Heading(1)
	productTitle.Append(compton.Fspan(p, title).TextAlign(align.Center))
	productTitle.SetAttribute("style", "view-transition-name:product-title-"+id)

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
			summaryRow.PropIcons(compton_data.PropertyTitles[property], osSymbols...).
				SetAttribute("style", "view-transition-name:"+property+id)
		default:
			summaryRow.PropVal(compton_data.PropertyTitles[property], strings.Join(values[property], ", ")).
				SetAttribute("style", "view-transition-name:"+property+id)
		}

	}
	pageStack.Append(compton.FICenter(p, summaryRow))

	/* Product details sections */

	hasSections := compton_fragments.ProductSections(id, rdx)

	for ii, section := range hasSections {

		sectionTitle := compton_data.SectionTitles[section]
		detailsSummary := compton.DSLarge(p, sectionTitle,
			slices.Contains(openSections, section)).
			BackgroundColor(color.Highlight).
			MarkerColor(color.RepGray).
			DetailsMarginBlockEnd(size.Unset).
			SummaryMarginBlockEnd(size.Normal)
		detailsSummary.SetId(section)
		detailsSummary.SetTabIndex(ii + 1)

		pageStack.Append(detailsSummary)

		switch section {
		case compton_data.InfoSection:
			productBadges := compton.FlexItems(p, direction.Row).
				ColumnGap(size.XSmall).
				BackgroundColor(color.Transparent)
			productBadges.SetAttribute("style", "view-transition-name:product-badges-"+id)
			for _, fmtBadge := range compton_fragments.FormatBadges(id, rdx) {

				var badge *compton.FspanElement
				if fmtBadge.Title != "" && fmtBadge.Icon == compton.NoSymbol {
					badge = compton.Badge(p, fmtBadge.Title, fmtBadge.Background, fmtBadge.Foreground)
				} else if fmtBadge.Icon != compton.NoSymbol {
					badge = compton.BadgeIcon(p, fmtBadge.Icon, "", fmtBadge.Background, fmtBadge.Foreground)
				}
				if badge != nil {
					badge.AddClass(fmtBadge.Class)
					productBadges.Append(badge)
				}
			}
			detailsSummary.AppendBadges(productBadges)
		case compton_data.MediaSection:
			var videos, images int
			if vp, sure := rdx.GetAllValues(vangogh_integration.VideoIdProperty, id); sure {
				videos = len(vp)
			}
			if sp, sure := rdx.GetAllValues(vangogh_integration.ScreenshotsProperty, id); sure {
				images = len(sp)
			}

			mediaCounts := make([]string, 0)
			if videos == 1 {
				mediaCounts = append(mediaCounts, "1 video")
			} else if videos > 1 {
				mediaCounts = append(mediaCounts, fmt.Sprintf("%d videos", videos))
			}
			if images == 1 {
				mediaCounts = append(mediaCounts, "1 image")
			} else if images > 1 {
				mediaCounts = append(mediaCounts, fmt.Sprintf("%d images", images))
			}

			if len(mediaCounts) > 0 {
				mediaBadge := compton.Badge(p, strings.Join(mediaCounts, ", "), color.Highlight, color.RepGray)
				detailsSummary.AppendBadges(mediaBadge)
			}

		case compton_data.CompatibilitySection:
			var badge compton.Element
			if dcp, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {

				var dcColor color.Color
				switch dcp {
				case "Verified":
					dcColor = color.Green
				case "Playable":
					dcColor = color.Orange
				case "Unsupported":
					dcColor = color.Red
				default:
					dcColor = color.RepGray
				}

				badge = compton.BadgeIcon(p, compton.Linux, dcp, color.Highlight, dcColor)
			} else if pt, ok := rdx.GetLastVal(vangogh_integration.ProtonDBTierProperty, id); ok {
				badge = compton.Badge(p, pt, color.Highlight, color.RepForeground)
			}

			if badge != nil {
				detailsSummary.AppendBadges(badge)
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
					receptionColor = color.RepGray
				}

				ratingsReviews := srep

				if srap, sure := rdx.GetLastVal(vangogh_integration.SummaryRatingProperty, id); sure {
					ratingsReviews = compton_fragments.FmtAggregatedRating(srap)
				}

				receptionBadges.Append(compton.Badge(p, ratingsReviews, color.Highlight, receptionColor))

			}

			detailsSummary.AppendBadges(receptionBadges)
		case compton_data.OfferingsSection:
			offerings := make([]string, 0)
			ops := []string{
				vangogh_integration.IsIncludedByGamesProperty,
				vangogh_integration.IsRequiredByGamesProperty,
				vangogh_integration.IsModifiedByGamesProperty,
				vangogh_integration.IncludesGamesProperty,
				vangogh_integration.RequiresGamesProperty,
				vangogh_integration.ModifiesGamesProperty}

			for _, op := range ops {
				if games, sure := rdx.GetAllValues(op, id); sure && len(games) > 0 {
					offerings = append(offerings, compton_data.PropertyTitles[op])
				}
			}

			if len(offerings) > 0 {
				offeringsBadge := compton.Badge(p, strings.Join(offerings, ", "), color.Highlight, color.RepGray)
				detailsSummary.AppendBadges(offeringsBadge)
			}
		case compton_data.NewsSection:

			if lcut, ok, err := rdx.ParseLastValTime(vangogh_integration.SteamLastCommunityUpdateProperty, id); ok && err == nil {

				updateColor := color.RepGray

				if (time.Since(lcut).Hours() / 24) < 30 {
					updateColor = color.Green
				}

				lastUpdateBadge := compton.Badge(p, lcut.Format("Jan 2, '06"), color.Highlight, updateColor)
				detailsSummary.AppendBadges(lastUpdateBadge)

			} else if err != nil {
				nod.LogError(err)
			}

		case compton_data.InstallersSection:
			if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
				pvr := vangogh_integration.ParseValidationResult(pvrs)

				validationBadge := compton.Badge(p,
					pvr.HumanReadableString(),
					color.Highlight,
					compton_fragments.ValidationResultsColors[pvr])
				detailsSummary.AppendBadges(validationBadge)
			}
		default:
			detailsSummary.SummaryMarginBlockEnd(size.Normal)
		}

		eagerness := loading.Lazy
		if section == compton_data.InfoSection {
			eagerness = loading.Eager
		}

		ifh := compton.IframeExpandHost(p, section, "/"+section+"?id="+id, eagerness)
		detailsSummary.Append(ifh)

	}

	if owned, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && owned == vangogh_integration.TrueValue {
		if productType, sure := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); sure && productType != vangogh_integration.GameProductType {

			hintSentences := []string{"No installers here."}
			switch productType {
			case vangogh_integration.DlcProductType:
				hintSentences = append(hintSentences, "Visit the <b>Offerings</b> section for the required product download links - including this DLC.")
			case vangogh_integration.PackProductType:
				hintSentences = append(hintSentences, "See the <b>Offerings</b> section for included products with downloads.")
			}

			nonGameInstallersHint := compton.Fspan(p, strings.Join(hintSentences, " ")).
				FontSize(size.Small).
				ForegroundColor(color.RepGray).
				TextAlign(align.Center)

			pageStack.Append(
				compton.Break(),
				compton.FICenter(p, nonGameInstallersHint))

		}
	}

	/* Standard app footer */

	pageStack.Append(compton.Br(),
		compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}
