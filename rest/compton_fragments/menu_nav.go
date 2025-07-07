package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"slices"
)

const navTitleMaxChars = 30

func MenuNav(r compton.Registrar, navTitle, id string, rdx redux.Readable) compton.Element {

	if len(navTitle) > navTitleMaxChars {
		navTitle = navTitle[:navTitleMaxChars] + "..."
	}

	dsMenu := compton.DSSmall(r, navTitle, false).SummaryJustifySelf(align.Center)
	dsMenu.SetId("menu-nav")
	dsMenu.SetAttribute("style", "view-transitions-name:menu-nav")

	r.RegisterStyles(compton_styles.Styles, "menu-nav.css")

	dsMenu.Append(menuNavItems(r, id, rdx))

	return dsMenu

}

func menuNavItems(r compton.Registrar, id string, rdx redux.Readable) compton.Element {

	pageStack := compton.FlexItems(r, direction.Column).JustifyContent(align.Center)

	// Updates

	updatesRow := compton.FlexItems(r, direction.Row).ColumnGap(size.Normal)
	pageStack.Append(updatesRow)

	updatesRow.Append(sectionNavLink(r, "Updates", "/updates"))

	sections := rdx.Keys(vangogh_integration.LastSyncUpdatesProperty)
	sortedKeys := slices.Sorted(sections)

	for _, section := range sortedKeys {
		if updates, ok := rdx.GetAllValues(vangogh_integration.LastSyncUpdatesProperty, section); ok && len(updates) > 0 {
			updatesSectionLink := navLink(r, section, "/updates?section="+section)
			updatesRow.Append(updatesSectionLink)
		}
	}

	// Search

	searchRow := compton.FlexItems(r, direction.Row).ColumnGap(size.Normal)
	pageStack.Append(searchRow)

	searchRow.Append(sectionNavLink(r, "Search", "/search"))

	searchScopes := compton_data.SearchScopes()

	for _, scope := range compton_data.SearchOrder {
		searchScopeLink := navLink(r, scope, "/search?"+searchScopes[scope])
		searchRow.Append(searchScopeLink)
	}

	// Product

	var productRow compton.Element

	if title, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		productRow = compton.FlexItems(r, direction.Row).ColumnGap(size.Normal)
		pageStack.Append(productRow)

		productRow.Append(sectionNavLink(r, title, "/product?id="+id))
	}

	for _, section := range ProductSections(id, rdx) {
		productSectionLink := navLink(r, compton_data.SectionTitles[section], "/product?id="+id+"#"+section)
		if productRow != nil {
			productRow.Append(productSectionLink)
		}
	}

	return pageStack
}

func sectionNavLink(r compton.Registrar, text string, href string) compton.Element {

	link := compton.A(href)

	linkText := compton.Fspan(r, text).
		FontSize(size.Small).
		ForegroundColor(color.RepGray)
	link.Append(linkText)

	return link

}

func navLink(r compton.Registrar, text string, href string) compton.Element {

	link := compton.A(href)

	linkText := compton.Fspan(r, text).
		FontSize(size.Small).
		FontWeight(font_weight.Bolder).
		ForegroundColor(color.RepForeground)
	link.Append(linkText)

	return link
}
