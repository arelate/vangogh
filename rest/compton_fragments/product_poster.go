package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
)

func ProductPoster(r compton.Registrar, id string, rdx redux.Readable) compton.Element {
	if imgSrc, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok {
		var poster compton.Element
		relImgSrc := "/image?id=" + imgSrc
		if dehydSrc, sure := rdx.GetLastVal(vangogh_integration.DehydratedImageProperty, id); sure {
			hydSrc := issa.HydrateColor(dehydSrc)
			repColor, _ := rdx.GetLastVal(vangogh_integration.RepImageColorProperty, id)
			issaImg := compton.IssaImageHydrated(r, repColor, hydSrc, relImgSrc)
			issaImg.AspectRatio(float64(13) / float64(6))
			poster = issaImg
		} else {
			poster = compton.IssaImageWrapper(r, relImgSrc)
		}
		poster.AddClass("product-poster")
		return poster
	}
	return nil
}
