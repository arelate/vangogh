package compton_fragments

import "github.com/boggydigital/compton"

func GogNavLinks(r compton.Registrar, current string) compton.Element {

	gogNavLinks := compton.NavLinks(r)

	gogNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/gog/search",
		Title:       "Search",
		IconElement: compton.SvgUse(r, compton.Search),
		Selected:    current == "Search",
	})

	gogNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/gog/owned",
		Title:       "Owned",
		IconElement: compton.SvgUse(r, compton.CircleCompactDisk),
		Selected:    current == "Owned",
	})

	gogNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/gog/catalog",
		Title:       "Catalog",
		IconElement: compton.SvgUse(r, compton.ShoppingLabel),
		Selected:    current == "Catalog",
	})

	gogNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/gog/wishlist",
		Title:       "Wishlist",
		IconElement: compton.SvgUse(r, compton.Heart),
		Selected:    current == "Wishlist",
	})

	gogNavLinks.AppendLink(r, &compton.NavTarget{
		Href:        "/gog/sale",
		Title:       "Sale",
		IconElement: compton.SvgUse(r, compton.Percent),
		Selected:    current == "Sale",
	})

	return gogNavLinks
}
