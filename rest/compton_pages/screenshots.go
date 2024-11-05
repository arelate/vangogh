package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/kevlar"
)

const eagerLoadingScreenshots = 3

func Screenshots(id string, rdx kevlar.ReadableRedux) compton.PageElement {

	screenshots, _ := rdx.GetAllValues(vangogh_local_data.ScreenshotsProperty, id)

	s := compton_fragments.ProductSection(compton_data.ScreenshotsSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if len(screenshots) == 0 {
		fs := compton.Fspan(s, "Screenshots are not available for this product").
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(s, fs))
	}

	for ii, src := range screenshots {
		imageSrc := "/image?id=" + src
		link := compton.A(imageSrc)
		link.SetAttribute("target", "_top")
		var img compton.Element
		if ii < eagerLoadingScreenshots {
			img = compton.ImgEager(imageSrc)
		} else {
			img = compton.ImgLazy(imageSrc)
		}
		link.Append(img)
		pageStack.Append(link)
	}

	return s
}
