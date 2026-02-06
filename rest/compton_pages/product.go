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
	"github.com/boggydigital/compton/consts/loading"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

var openSections = []string{
	//compton_data.InfoSection,
}

var offeringsBadgesOrder = []compton.Symbol{
	compton.CircleCompactDisk,
	compton.ItemsPack,
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
	//productTitle.SetAttribute("style", "view-transition-name:product-title-"+id)

	/* Product labels */

	pageStack.Append(compton.FICenter(p, productTitle))

	/* Product summary properties */

	properties, values := compton_fragments.SummarizeProductProperties(id, rdx)
	osSymbols := make([]compton.Symbol, 0, 2)

	summaryRow := compton.Frow(p).BackgroundColor(color.Transparent)
	summaryRow.FontSize(size.XSmall)

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
			for _, value := range values[property] {
				href := "/search?sort=global-release-date&desc=true&" + property + "=" + value
				summaryRow.PropLinkColor(compton_data.PropertyTitles[property], color.RepForeground, value, href)
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
		detailsSummary.SetAccessKey(compton_data.SectionAccessKeys[section])

		pageStack.Append(detailsSummary)

		switch section {
		case compton_data.InfoSection:

			fmtBadges := compton_fragments.FormatBadges(id, rdx, compton_data.InformationBadgeProperties, permissions...)

			productBadges := compton.Badges(p, fmtBadges...)

			//productBadges.SetAttribute("style", "view-transition-name:product-badges-"+id)

			detailsSummary.AppendBadges(productBadges)
		case compton_data.MediaSection:
			var videos, images int
			if vp, sure := rdx.GetAllValues(vangogh_integration.VideoIdProperty, id); sure {
				videos = len(vp)
			}
			if sp, sure := rdx.GetAllValues(vangogh_integration.ScreenshotsProperty, id); sure {
				images = len(sp)
			}

			var fmtMediaBadges []compton.FormattedBadge

			if videos > 0 {
				fmtMediaBadges = append(fmtMediaBadges, compton.FormattedBadge{
					Icon:  compton.VideoThumbnail,
					Title: strconv.Itoa(videos),
					Color: color.RepGray,
				})
			}

			if images > 0 {
				fmtMediaBadges = append(fmtMediaBadges, compton.FormattedBadge{
					Icon:  compton.ImageThumbnail,
					Title: strconv.Itoa(images),
					Color: color.RepGray,
				})
			}

			if videos+images > 0 {
				detailsSummary.AppendBadges(compton.Badges(p, fmtMediaBadges...))
			}

		case compton_data.CompatibilitySection:

			var dcText string
			dcColor := color.RepGray
			dcSymbol := compton.NoSymbol

			if dcp, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {
				dcText = dcp
				if dcs, sure := compton_data.CompatibilitySymbols[dcp]; sure {
					dcSymbol = dcs
				}
				if dcc, sure := compton_data.CompatibilityColors[dcp]; sure {
					dcColor = dcc
				}
			}

			if dcText == "" || dcText == "Unknown" {
				if pt, ok := rdx.GetLastVal(vangogh_integration.ProtonDBTierProperty, id); ok {
					dcText = pt
					if dcs, sure := compton_data.CompatibilitySymbols[pt]; sure {
						dcSymbol = dcs
					}
					if dcc, sure := compton_data.CompatibilityColors[pt]; sure {
						dcColor = dcc
					}
				}
			}

			fmtCompatBadge := compton.FormattedBadge{
				Title: dcText,
				Icon:  dcSymbol,
				Color: dcColor,
			}

			detailsSummary.AppendBadges(compton.Badges(p, fmtCompatBadge))

		case compton_data.ReceptionSection:

			var fmtReceptionBadges []compton.FormattedBadge

			if tp, sure := rdx.GetLastVal(vangogh_integration.TopPercentProperty, id); sure && tp != "" {
				fmtReceptionBadges = append(fmtReceptionBadges, compton.FormattedBadge{
					Title: tp,
					Icon:  compton.Trophy,
					Color: color.Green,
				})
			}

			receptionSymbol := compton.NoSymbol
			receptionColor := color.RepGray

			if srep, ok := rdx.GetLastVal(vangogh_integration.SummaryReviewsProperty, id); ok {

				if rs, sure := compton_data.ReceptionSymbols[srep]; sure {
					receptionSymbol = rs
				}

				if rc, sure := compton_data.ReceptionColors[srep]; sure {
					receptionColor = rc
				}

				ratingsReviews := srep

				if srap, sure := rdx.GetLastVal(vangogh_integration.SummaryRatingProperty, id); sure {
					ratingsReviews = compton_fragments.FmtRatingValue(srap)
				}

				fmtReceptionBadges = append(fmtReceptionBadges, compton.FormattedBadge{
					Title: ratingsReviews,
					Icon:  receptionSymbol,
					Color: receptionColor,
				})

			}

			detailsSummary.AppendBadges(compton.Badges(p, fmtReceptionBadges...))
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

				fmtOfferingsBadges := make([]compton.FormattedBadge, 0, len(offeringsSymbols))

				for _, os := range offeringsBadgesOrder {
					if count, sure := offeringsSymbols[os]; sure {
						fmtOfferingsBadges = append(fmtOfferingsBadges, compton.FormattedBadge{
							Title: strconv.Itoa(count),
							Icon:  os,
							Color: color.RepGray,
						})
					}
				}

				detailsSummary.AppendBadges(compton.Badges(p, fmtOfferingsBadges...))
			}
		case compton_data.NewsSection:

			if lcut, ok, err := rdx.ParseLastValTime(vangogh_integration.SteamLastCommunityUpdateProperty, id); ok && err == nil {

				updateColor := color.RepGray

				if (time.Since(lcut).Hours() / 24) < 30 {
					updateColor = color.Green
				}

				fmtNewsBadge := compton.FormattedBadge{
					Title: lcut.Format("Jan 2, '06"),
					Icon:  compton.NewsBroadcast,
					Color: updateColor,
				}

				detailsSummary.AppendBadges(compton.Badges(p, fmtNewsBadge))

			} else if err != nil {
				nod.LogError(err)
			}

		case compton_data.InstallersSection:

			productDvs := vangogh_integration.NewProductDvs(id, rdx)

			productDownloadStatus := productDvs.DownloadStatus()
			productValidationStatus := productDvs.ValidationStatus()

			statusColor := color.RepGray
			statusSymbol := compton.NoSymbol

			switch productValidationStatus {
			case vangogh_integration.ValidationStatusUnknown:
				if dss, sure := compton_data.DownloadStatusSymbols[productDownloadStatus]; sure {
					statusSymbol = dss
				}
			default:
				statusColor = compton_data.ValidationStatusColors[productValidationStatus]
				if vss, sure := compton_data.ValidationStatusSymbols[productValidationStatus]; sure {
					statusSymbol = vss
				}
			}

			fmtPvrBadge := compton.FormattedBadge{
				Title: productDvs.HumanReadableString(),
				Icon:  statusSymbol,
				Color: statusColor,
			}

			var badges []compton.FormattedBadge
			badges = append(badges, fmtPvrBadge)

			if pgs, sure := rdx.GetLastVal(vangogh_integration.ProductGeneratedChecksumProperty, id); sure && pgs == vangogh_integration.TrueValue {
				generatedChecksumBadge := compton.FormattedBadge{
					Title: "Generated Checksum",
					Color: color.Yellow,
				}
				badges = append(badges, generatedChecksumBadge)
			}

			detailsSummary.AppendBadges(compton.Badges(p, badges...))

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
