package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
)

func Videos(videoIds []string, videoTitles, videoDurations map[string]string) compton.Element {

	s := compton_fragments.ProductSection(compton_data.VideosSection)

	pageStack := flex_items.FlexItems(s, direction.Column)

	s.Append(pageStack)

	if len(videoIds) == 0 {
		fs := fspan.Text(s, "Videos are not available for this product").
			ForegroundColor(color.Gray)
		pageStack.Append(flex_items.Center(s, fs))
	}

	for ii, videoId := range videoIds {
		videoTitle := videoTitles[videoId]
		videoDuration := videoDurations[videoId]
		pageStack.Append(compton_fragments.VideoOriginLink(s, videoId, videoTitle, videoDuration))

		if ii != len(videoIds)-1 {
			pageStack.Append(els.Hr())
		}
	}

	return s
}
