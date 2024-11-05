package compton_fragments

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/kevlar"
)

func ProductPoster(r compton.Registrar, id string, rdx kevlar.ReadableRedux) compton.Element {
	if imgSrc, ok := rdx.GetLastVal(vangogh_local_data.ImageProperty, id); ok {
		var poster compton.Element
		relImgSrc := "/image?id=" + imgSrc
		if dehydSrc, sure := rdx.GetLastVal(vangogh_local_data.DehydratedImageProperty, id); sure {
			hydSrc := issa.HydrateColor(dehydSrc)
			repColor, _ := rdx.GetLastVal(vangogh_local_data.RepImageColorProperty, id)
			issaImg := compton.IssaImageHydrated(r, repColor, hydSrc, relImgSrc)
			issaImg.AspectRatio(float64(13) / float64(6))
			poster = issaImg
		} else {
			poster = compton.Img(relImgSrc)
		}
		poster.AddClass("product-poster")
		return poster
	}
	return nil
}
