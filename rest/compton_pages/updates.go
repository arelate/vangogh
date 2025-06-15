package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"maps"
	"slices"
)

func Updates(section string,
	updates map[string][]string,
	updateTotals map[string]int,
	updated string,
	rdx redux.Readable) compton.PageElement {

	current := compton_data.AppNavUpdates
	p, pageStack := compton_fragments.AppPage(current)

	p.AppendSpeculationRules("/product?id=*")

	p.RegisterStyles(compton_styles.Styles, "product-card.css")
	p.RegisterStyles(compton_styles.Styles, "good-old-game-badge.css")

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	/* Nav stack = App navigation + Show all + (popup) Updates sections shortcuts */

	topLevelNav := []compton.Element{compton_fragments.AppNavLinks(p, current)}

	updateSectionLinks := compton.NavLinks(p)
	updateSectionLinks.SetAttribute("style", "view-transition-name:secondary-nav")

	for _, updateSection := range slices.Sorted(maps.Keys(updates)) {

		sectionLink := updateSectionLinks.AppendLink(p, &compton.NavTarget{
			Href:     "/updates?section=" + updateSection,
			Title:    updateSection,
			Selected: updateSection == section,
		})

		if updateSection == section {
			sectionLink.SetAttribute("style", "view-transition-name:current-update-section")
		}

	}

	topLevelNav = append(topLevelNav, updateSectionLinks)

	var showAllNavLinks *compton.NavLinksElement

	if len(updates[section]) < updateTotals[section] {
		showAllNavLinks = compton.NavLinks(p)
		showAllNavLinks.AppendLink(p, &compton.NavTarget{Href: "/updates?section=" + section + "&show-all=true", Title: "Show all"})

		topLevelNav = append(topLevelNav, showAllNavLinks)
	}

	pageStack.Append(compton.FICenter(p, topLevelNav...))

	/* Updates sections */

	ids := updates[section]

	//sectionHeading := compton.DSTitle(p, section)

	dsSection := compton.DSLarge(p, section, true).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	cf := compton.NewCountFormatter(
		compton_data.SingleItemTemplate,
		compton_data.ManyItemsSinglePageTemplate,
		compton_data.ManyItemsManyPagesTemplate)

	itemsBadge := compton.Badge(p, cf.Title(0, len(ids), updateTotals[section]), color.Background, color.Foreground)
	dsSection.AppendBadges(itemsBadge)

	dsSection.SetId(section)
	pageStack.Append(dsSection)

	sectionStack := compton.FlexItems(p, direction.Column)
	dsSection.Append(sectionStack)

	productsList := compton_fragments.ProductsList(p, ids, 0, len(ids), rdx, false)
	sectionStack.Append(productsList)

	if showAllNavLinks != nil {
		pageStack.Append(compton.FICenter(p, showAllNavLinks))
	}

	/* Last Updated section */

	pageStack.Append(compton.Br(), compton_fragments.Updated(p, updated))

	/* Standard app footer */

	pageStack.Append(compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}
