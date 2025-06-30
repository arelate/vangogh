package compton_fragments

import (
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"strconv"
)

const dehydratedCount = 3

func ProductsList(r compton.Registrar, ids []string, from, to int, rdx redux.Readable, topTarget bool) compton.Element {

	r.RegisterStyles(compton_styles.Styles, "products-list.css")

	productCards := compton.FlexItems(r, direction.Row).JustifyContent(align.Center).Width(size.FullWidth)
	productCards.AddClass("products-list")

	if (to - from) < 10 {
		productCards.AddClass("items-" + strconv.Itoa(to-from))
	}

	for ii := from; ii < to; ii++ {
		id := ids[ii]
		productLink := compton.A("/product?id=" + id)
		if topTarget {
			productLink.SetAttribute("target", "_top")
		}

		if productCard := ProductCard(r, id, ii-from < dehydratedCount, rdx); productCard != nil {
			productLink.Append(productCard)
			productCards.Append(productLink)
		}
	}

	return productCards
}
