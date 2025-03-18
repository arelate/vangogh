package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/redux"
)

const dehydratedCount = 3

func ProductsList(r compton.Registrar, ids []string, from, to int, rdx redux.Readable) compton.Element {
	productCards := compton.GridItems(r).JustifyContent(align.Center)

	for ii := from; ii < to; ii++ {
		id := ids[ii]
		productLink := compton.A("/product?id=" + id)

		if productCard := ProductCard(r, id, ii-from < dehydratedCount, rdx); productCard != nil {
			productLink.Append(productCard)
			productCards.Append(productLink)
		}
	}

	return productCards
}
