package compton_pages

import (
	"errors"
	"slices"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

const (
	updatedProductsLimit = 60 // divisible by 2,3,4,5,6
)

func Updates(section string, rdx redux.Readable, showAll bool, permissions ...author.Permission) compton.PageElement {

	updates := make(map[string][]string)
	updateTotals := make(map[string]int)

	paginate := false

	var month time.Month
	var day int

	if sdt, ok := rdx.GetLastVal(vangogh_integration.SyncEventsProperty, vangogh_integration.SyncDataKey); ok {
		if sdi, err := strconv.ParseInt(sdt, 10, 64); err == nil {
			_, month, day = time.Unix(sdi, 0).Date()
		}
	}

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
		for _, us := range vangogh_integration.UpdatesOrder {
			if _, ok := updates[us]; ok {
				if prm, ok := compton_data.UpdateSectionPermissions[us]; ok && !slices.Contains(permissions, prm) {
					continue
				}
				section = us
				break
			}
		}
	}

	current := compton_data.AppNavUpdates
	p, pageStack := compton_fragments.AppPage(current)

	if section != "" {
		if prm, ok := compton_data.UpdateSectionPermissions[section]; ok && !slices.Contains(permissions, prm) {
			p.Error(errors.New("section access restricted"))
			return p
		}
	}

	p.AppendSpeculationRules(compton.SpeculationRulesConservativeEagerness, "/*")

	p.SetAttribute("style", "--c-rep:var(--c-background)")

	/* Nav stack = App navigation + Show all + (popup) Updates sections shortcuts */

	topLevelNav := []compton.Element{compton_fragments.AppNavLinks(p, current)}

	updateSectionLinks := compton.NavLinks(p)

	for _, updateSection := range vangogh_integration.UpdatesOrder {

		if _, ok := updates[updateSection]; !ok {
			continue
		}

		if prm, ok := compton_data.UpdateSectionPermissions[updateSection]; ok && !slices.Contains(permissions, prm) {
			continue
		}

		var sectionSymbol compton.Symbol
		if symbol, ok := compton_data.UpdateSectionSymbols[updateSection]; ok {
			sectionSymbol = symbol
		}

		var iconElement compton.Element

		switch updateSection {
		case vangogh_integration.UpdatesReleasedToday:
			iconElement = compton.MonthDay(p, month, day)
		default:
			iconElement = compton.SvgUse(p, sectionSymbol)
		}

		updateSectionLinks.AppendLink(p, &compton.NavTarget{
			Href:        "/updates?section=" + updateSection,
			Title:       vangogh_integration.UpdatesShorterTitles[updateSection],
			IconElement: iconElement,
			Selected:    updateSection == section,
		})

	}

	if len(updates) > 0 {
		topLevelNav = append(topLevelNav, updateSectionLinks)
	}

	pageStack.Append(compton.FICenter(p, topLevelNav...))

	/* Updates sections */

	ids := updates[section]

	sectionTitle := vangogh_integration.UpdatesLongerTitles[section]

	switch section {
	case vangogh_integration.UpdatesReleasedToday:
		sectionTitle = strings.Replace(sectionTitle, "{month}", month.String(), 1)
		sectionTitle = strings.Replace(sectionTitle, "{day}", strconv.FormatInt(int64(day), 10), 1)
	default:
		// do nothing
	}

	dsSection := compton.DSLarge(p, sectionTitle, true).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset).
		SummaryRowGap(size.XXSmall)

	cf := compton.NewCountFormatter(
		compton_data.SingleItemTemplate,
		compton_data.ManyItemsSinglePageTemplate,
		compton_data.ManyItemsManyPagesTemplate)

	dsSection.AppendBadges(compton.Badges(p, &compton.FormattedBadge{
		Title: cf.Title(0, len(ids), updateTotals[section]),
		Icon:  compton.NoSymbol,
		Color: color.Foreground,
	}))

	dsSection.SetId(section)

	if len(updates) > 0 {
		pageStack.Append(dsSection)
	} else {
		pageStack.Append(compton.Br(), compton.FICenter(p,
			compton.Fspan(p, "No recent updates.").ForegroundColor(color.Yellow)))
	}

	sectionStack := compton.FlexItems(p, direction.Column)
	dsSection.Append(sectionStack)

	productsList := compton_fragments.ProductsList(p, ids, 0, len(ids), rdx, false, permissions...)
	sectionStack.Append(productsList)

	/* Show all */

	if len(updates[section]) < updateTotals[section] {
		showAllNavLinks := compton.NavLinks(p)
		//showAllNavLinks.SetAttribute("style", "view-transition-name:tertiary-nav")
		showAllNavLinks.AppendLink(p, &compton.NavTarget{Href: "/updates?section=" + section + "&all", Title: "Show all"})

		backToTopNavLinks := compton.NavLinks(p)
		backToTopNavLinks.AppendLink(p, &compton.NavTarget{Href: "#_top", Title: "Back to top"})

		pageStack.Append(compton.FICenter(p, backToTopNavLinks, showAllNavLinks).ColumnGap(size.Small))
	}

	/* Last Updated section */

	pageStack.Append(compton.Br(), compton_fragments.SyncStatus(p, rdx, permissions...))

	/* Standard app footer */

	pageStack.Append(compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}
