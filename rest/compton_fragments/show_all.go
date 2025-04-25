package compton_fragments

import "github.com/boggydigital/compton"

func ShowAll(r compton.Registrar) *compton.NavLinksElement {
	showAllNavLinks := compton.NavLinks(r)

	showAllLinkListItem := compton.ListItem()

	showTocLink := compton.AText("Show all", "?show-all=true")

	showAllLinkListItem.Append(showTocLink)

	showAllNavLinks.Append(showAllLinkListItem)

	return showAllNavLinks
}
