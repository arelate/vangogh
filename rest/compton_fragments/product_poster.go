package compton_fragments

import (
	"fmt"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func ProductPoster(id string, rdx redux.Readable) compton.Element {
	if imgSrc, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok {
		relImgSrc := "/image?id=" + imgSrc

		imgLazy := compton.ImageEager(relImgSrc)
		imgLazy.SetAttribute("style", "aspect-ratio:"+fmt.Sprintf("%f", float64(13)/float64(6)))
		imgLazy.AddClass("product-poster")

		return imgLazy
	}
	return nil
}
