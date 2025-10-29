package compton_pages

import (
	_ "embed"
	"slices"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/loading"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

var openSections = []string{
	compton_data.InfoSection,
}

var offeringsBadgesOrder = []compton.Symbol{
	compton.CompactDisk,
	compton.TwoStackedItems,
	compton.ItemPlus,
	compton.PuzzlePiece,
}

func Product(id string, rdx redux.Readable, permissions ...author.Permission) compton.PageElement {

	title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id)
	if !ok {
		return nil
	}

	p, pageStack := compton_fragments.AppPage(title)

	p.AppendSpeculationRules(compton.SpeculationRulesConservativeEagerness, "/*")

	p.RegisterStyles(compton_styles.Styles, "product.css")

	// tinting document background color to the representative product color
	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if repColor, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && repColor != issa.NeutralRepColor {
			p.SetAttribute("style", "--c-rep:"+repColor)
		}
	}

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(compton.FICenter(p, poster))
	}

	/* Product title */

	productTitle := compton.Heading(2)
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
			for _, value := range values[property] {
				href := "/search?sort=global-release-date&desc=true&" + property + "=" + value
				summaryRow.PropLinkColor(compton_data.PropertyTitles[property], color.RepForeground, value, href).
					SetAttribute("style", "view-transition-name:"+property+id)
			}
		}

	}
	pageStack.Append(compton.FICenter(p, summaryRow))

	/* Product details sections */

	hasSections := compton_fragments.ProductSections(id, rdx, permissions...)

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
				ColumnGap(size.Small).
				FontSize(size.XXSmall).
				BackgroundColor(color.Transparent).
				JustifyItems(align.Center).
				AlignItems(align.Center)

			productBadges.SetAttribute("style", "view-transition-name:product-badges-"+id)
			for _, fmtBadge := range compton_fragments.FormatBadges(id, rdx, compton_data.InformationBadgeProperties, permissions...) {

				var badge compton.Element
				switch fmtBadge.Title {
				case "":
					badge = compton.BadgeIcon(p, fmtBadge.Icon, color.RepGray)
				default:
					badge = compton.BadgeText(p, fmtBadge.Title, color.RepGray)
				}
				productBadges.Append(badge)
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

			mediaBadges := compton.FlexItems(p, direction.Row).ColumnGap(size.Small).FontSize(size.XXSmall)

			if videos > 0 {
				mediaBadges.Append(
					compton.BadgeIcon(p, compton.VideoThumbnail, color.RepGray),
					compton.BadgeText(p, strconv.Itoa(videos), color.RepGray))
			}

			if images > 0 {
				mediaBadges.Append(
					compton.BadgeIcon(p, compton.ImageThumbnail, color.RepGray),
					compton.BadgeText(p, strconv.Itoa(images), color.RepGray))
			}

			if videos+images > 0 {
				detailsSummary.AppendBadges(mediaBadges)
			}

		case compton_data.CompatibilitySection:
			var compatText string
			if dcp, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {
				compatText = dcp
			}

			if compatText == "" || compatText == "Unknown" {
				if pt, ok := rdx.GetLastVal(vangogh_integration.ProtonDBTierProperty, id); ok {
					compatText = pt
				}
			}

			var dcColor color.Color
			var dcSymbol compton.Symbol
			switch compatText {
			case "Platinum":
				fallthrough
			case "Gold":
				fallthrough
			case "Verified":
				dcColor = color.Green
				dcSymbol = compton.Circle
			case "Silver":
				fallthrough
			case "Bronze":
				fallthrough
			case "Playable":
				dcColor = color.Orange
				dcSymbol = compton.Triangle
			case "Borked":
				fallthrough
			case "Unsupported":
				dcColor = color.Red
				dcSymbol = compton.Cross
			default:
				dcColor = color.RepGray
				dcSymbol = compton.NoSymbol
			}

			compatibilityBadges := compton.FlexItems(p, direction.Row).
				ColumnGap(size.Small).
				FontSize(size.XXSmall).
				BackgroundColor(color.Transparent).
				JustifyItems(align.Center).
				AlignItems(align.Center)

			compatibilityBadges.Append(
				compton.BadgeIcon(p, dcSymbol, dcColor),
				compton.BadgeText(p, compatText, dcColor))

			detailsSummary.AppendBadges(compatibilityBadges)

		case compton_data.ReceptionSection:
			receptionBadges := compton.FlexItems(p, direction.Row).ColumnGap(size.Small).FontSize(size.XXSmall)

			if tp, sure := rdx.GetLastVal(vangogh_integration.TopPercentProperty, id); sure && tp != "" {
				receptionBadges.Append(
					compton.BadgeIcon(p, compton.Trophy, color.Green),
					compton.BadgeText(p, tp, color.Green))
			}

			var receptionSymbol compton.Symbol

			if srep, ok := rdx.GetLastVal(vangogh_integration.SummaryReviewsProperty, id); ok {

				var receptionColor color.Color
				switch srep {
				case vangogh_integration.RatingPositive:
					receptionSymbol = compton.ThreeUpwardChevrons
					receptionColor = color.Green
				case vangogh_integration.RatingNegative:
					receptionSymbol = compton.ThreeDownwardChevrons
					receptionColor = color.Red
				case vangogh_integration.RatingMixed:
					receptionSymbol = compton.ThreeHorizontalLines
					receptionColor = color.Yellow
				default:
					receptionColor = color.RepGray
				}

				ratingsReviews := srep

				if srap, sure := rdx.GetLastVal(vangogh_integration.SummaryRatingProperty, id); sure {
					ratingsReviews = compton_fragments.FmtRatingValue(srap)
				}

				receptionBadges.Append(
					compton.BadgeIcon(p, receptionSymbol, receptionColor),
					compton.BadgeText(p, ratingsReviews, receptionColor))

			}

			detailsSummary.AppendBadges(receptionBadges)
		case compton_data.OfferingsSection:
			ops := []string{
				vangogh_integration.IsIncludedByGamesProperty,
				vangogh_integration.IsRequiredByGamesProperty,
				vangogh_integration.IsModifiedByGamesProperty,
				vangogh_integration.IncludesGamesProperty,
				vangogh_integration.RequiresGamesProperty,
				vangogh_integration.ModifiesGamesProperty}

			offeringsSymbols := make(map[compton.Symbol]int)

			for _, op := range ops {
				if games, sure := rdx.GetAllValues(op, id); sure && len(games) > 0 {
					offeringsSymbols[compton_data.PropertySymbols[op]] += len(games)
				}
			}

			if len(offeringsSymbols) > 0 {
				offeringsBadgesRow := compton.FlexItems(p, direction.Row).ColumnGap(size.Small).FontSize(size.XXSmall)

				for _, os := range offeringsBadgesOrder {
					if count, sure := offeringsSymbols[os]; sure {
						offeringsBadgesRow.Append(
							compton.BadgeIcon(p, os, color.RepGray),
							compton.BadgeText(p, strconv.Itoa(count), color.RepGray))
					}
				}

				detailsSummary.AppendBadges(offeringsBadgesRow)
			}
		case compton_data.NewsSection:

			if lcut, ok, err := rdx.ParseLastValTime(vangogh_integration.SteamLastCommunityUpdateProperty, id); ok && err == nil {

				updateColor := color.RepGray

				if (time.Since(lcut).Hours() / 24) < 30 {
					updateColor = color.Green
				}

				updateBadges := compton.FlexItems(p, direction.Row).ColumnGap(size.Small).FontSize(size.XXSmall)

				updateBadges.Append(
					compton.BadgeIcon(p, compton.NewsBroadcast, updateColor),
					compton.BadgeText(p, lcut.Format("Jan 2, '06"), updateColor))

				detailsSummary.AppendBadges(updateBadges)

			} else if err != nil {
				nod.LogError(err)
			}

		case compton_data.InstallersSection:
			if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
				pvr := vangogh_integration.ParseValidationResult(pvrs)

				validationBadgesRow := compton.FlexItems(p, direction.Row).ColumnGap(size.Small).FontSize(size.XXSmall)

				vrColor := compton_data.ValidationResultsColors[pvr]

				if vrSymbol, sure := compton_data.ValidationResultsSymbols[pvr]; sure {
					badgeSymbol := compton.BadgeIcon(p, vrSymbol, vrColor)
					validationBadgesRow.Append(badgeSymbol)
				}

				validationBadgesRow.Append(compton.BadgeText(p, pvr.HumanReadableString(), vrColor))

				detailsSummary.AppendBadges(validationBadgesRow)
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

		var hintSentences []string

		if preorder, sure := rdx.GetLastVal(vangogh_integration.PreOrderProperty, id); sure && preorder == vangogh_integration.TrueValue {
			hintSentences = append(hintSentences, "Installers are not available for pre-orders.")
		}

		if productType, sure := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); sure && productType != vangogh_integration.GameProductType {

			if len(hintSentences) == 0 {
				hintSentences = append(hintSentences, "No installers here.")
			}

			switch productType {
			case vangogh_integration.DlcProductType:
				hintSentences = append(hintSentences, "Visit the <b>Offerings</b> section for the required product download links - including this DLC.")
			case vangogh_integration.PackProductType:
				hintSentences = append(hintSentences, "See the <b>Offerings</b> section for included products with downloads.")
			}

		}

		if len(hintSentences) > 0 {
			noInstallersHint := compton.Fspan(p, strings.Join(hintSentences, " ")).
				FontSize(size.Small).
				ForegroundColor(color.RepGray).
				TextAlign(align.Center)

			pageStack.Append(
				compton.Break(),
				compton.FICenter(p, noInstallersHint))
		}

	}

	/* Standard app footer */

	var footerLinks []compton.Element

	if slices.Contains(permissions, perm.ReadDebug) {
		footerLinks = append(footerLinks, compton_fragments.DebugLink(p, id))
	}
	footerLinks = append(footerLinks, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p))

	pageStack.Append(compton.Br(), compton.FICenter(p, footerLinks...))

	return p
}
