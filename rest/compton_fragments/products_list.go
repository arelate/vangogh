package compton_fragments

import (
	"path"

	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func GogProductsList(r compton.Registrar, ids []string, from, to int, rdx redux.Readable, topTarget bool, permissions ...author.Permission) compton.Element {

	r.RegisterStyles(compton_styles.Styles, "products-list.css")

	productCards := compton.FlexItems(r, direction.Row).JustifyContent(align.Center).Width(size.FullWidth)
	productCards.AddClass("products-list")

	for ii := from; ii < to; ii++ {
		id := ids[ii]
		productLink := compton.A(path.Join("/gog-product", id))
		if topTarget {
			productLink.SetAttribute("target", "_top")
		}

		if productCard := GogProductCard(r, id, rdx, permissions...); productCard != nil {
			productLink.Append(productCard)
			productCards.Append(productLink)
		}
	}

	return productCards
}
