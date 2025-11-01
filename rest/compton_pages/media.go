package compton_pages

import (
	"slices"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/redux"
)

const eagerLoadingScreenshots = 3

func Media(id string, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.MediaSection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column)

	s.Append(pageStack)

	var videoIds []string
	if vids, ok := rdx.GetAllValues(vangogh_integration.VideoIdProperty, id); ok {
		videoIds = vids
	}
	slices.Sort(videoIds)

	videoTitles := make(map[string]string)
	videoDurations := make(map[string]string)

	for _, vid := range videoIds {

		if vtp, ok := rdx.GetLastVal(vangogh_integration.VideoTitleProperty, vid); ok {
			videoTitles[vid] = vtp
		}
		if vdp, ok := rdx.GetLastVal(vangogh_integration.VideoDurationProperty, vid); ok {
			videoDurations[vid] = vdp
		}
	}

	if len(videoIds) == 0 {
		fs := compton.Fspan(s, "Videos are not available for this product").
			ForegroundColor(color.RepGray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(s, fs))
	}

	for ii, videoId := range videoIds {
		videoTitle := videoTitles[videoId]
		videoDuration := videoDurations[videoId]
		pageStack.Append(compton_fragments.VideoOriginLink(s, videoId, videoTitle, videoDuration))

		if ii != len(videoIds)-1 {
			pageStack.Append(compton.Hr())
		}
	}

	var screenshots []string
	if sp, ok := rdx.GetAllValues(vangogh_integration.ScreenshotsProperty, id); ok {
		screenshots = sp
	}

	if len(videoIds) > 0 && len(screenshots) > 0 {

		fmtBadge := compton.FormattedBadge{
			Title: "Screenshots",
			Icon:  compton.ImageThumbnail,
			Color: color.RepForeground,
		}

		pageStack.Append(compton.SectionDivider(s, fmtBadge))
	}

	if len(screenshots) == 0 {
		fs := compton.Fspan(s, "Screenshots are not available for this product").
			ForegroundColor(color.RepGray).
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
