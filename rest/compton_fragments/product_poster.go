package compton_fragments

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/issa_image"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/kevlar"
)

func ProductPoster(r compton.Registrar, id string, rdx kevlar.ReadableRedux) compton.Element {
	if imgSrc, ok := rdx.GetLastVal(vangogh_local_data.ImageProperty, id); ok {
		var poster compton.Element
		relImgSrc := "/image?id=" + imgSrc
		if dehydSrc, sure := rdx.GetLastVal(vangogh_local_data.DehydratedImageProperty, id); sure {
			hydSrc := issa.HydrateColor(dehydSrc)
			poster = issa_image.IssaImageHydrated(r, hydSrc, relImgSrc)
		} else {
			poster = els.Img(relImgSrc)
		}
		poster.AddClass("product-poster")
		return poster
	}
	return nil
}
