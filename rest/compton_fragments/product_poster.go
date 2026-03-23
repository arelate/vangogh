package compton_fragments

import (
	"fmt"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/redux"
)

func ProductPoster(id string, rdx redux.Readable) compton.Element {
	if imgSrc, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imgSrc != "" {
		relImgSrc := "/image?id=" + imgSrc

		imgEager := compton.ImageEager(relImgSrc)
		imgEager.SetAttribute("fetchpriority", "high")
		imgEager.SetAttribute("style", "aspect-ratio:"+fmt.Sprintf("%f", float64(13)/float64(6)))

		imgEager.AddClass("product-poster")

		return imgEager
	}
	return nil
}
