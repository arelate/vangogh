package compton_pages

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"maps"
	"slices"
)

const (
	updatedProductsLimit = 60 // divisible by 2,3,4,5,6
)

func Updates(section string, rdx redux.Readable, showAll bool) compton.PageElement {

	updates := make(map[string][]string)
	updateTotals := make(map[string]int)

	paginate := false

	for updateSection := range rdx.Keys(vangogh_integration.LastSyncUpdatesProperty) {

		ids, _ := rdx.GetAllValues(vangogh_integration.LastSyncUpdatesProperty, updateSection)
		updateTotals[updateSection] = len(ids)

		paginate = len(ids) > updatedProductsLimit
		for _, id := range ids {
			if paginate && !showAll && len(updates[updateSection]) >= updatedProductsLimit {
				continue
			}
			updates[updateSection] = append(updates[updateSection], id)
		}
	}

	keys := make(map[string]bool)
	for _, ids := range updates {
		for _, id := range ids {
			keys[id] = true
		}
	}

	if section == "" {
		if sortedSections := slices.Sorted(maps.Keys(updates)); len(sortedSections) > 0 {
			section = sortedSections[0]
		}
	}

	current := compton_data.AppNavUpdates
	p, pageStack := compton_fragments.AppPage(current)

	p.AppendSpeculationRules("/*")

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	/* Nav stack = App navigation + Show all + (popup) Updates sections shortcuts */

	menuNavLink := compton_fragments.MenuNav(p, section, "", rdx)
	pageStack.Append(menuNavLink)

	/* Updates sections */

	ids := updates[section]

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

	/* Last Updated section */

	pageStack.Append(compton.Br(), compton_fragments.Updated(p, rdx))

	/* Standard app footer */

	pageStack.Append(compton.Footer(p, "Arles", "https://github.com/arelate", "🇫🇷"))

	return p
}
