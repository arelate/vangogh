package compton_fragments

import "github.com/boggydigital/compton"

func ShowToc(r compton.Registrar) (*compton.NavLinksElement, compton.Element) {
	showTocNavLinks := compton.NavLinks(r)

	showTocLinkListItem := compton.ListItem()

	showTocLink := compton.A("#")
	showTocLink.Append(compton.SvgUse(r, compton.DownwardArrow))

	showTocLinkListItem.Append(showTocLink)

	showTocNavLinks.Append(showTocLinkListItem)

	return showTocNavLinks, showTocLink
}
