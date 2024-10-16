package compton_fragments

import (
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
	"github.com/boggydigital/yet_urls/youtube_urls"
	"strconv"
	"time"
)

func formatSeconds(ts int64) string {
	if ts == 0 {
		return "unknown"
	}

	t := time.Unix(ts, 0).UTC()

	layout := "4:05"
	if t.Hour() > 0 {
		layout = "15:04:05"
	}

	return t.Format(layout)
}

func VideoOriginLink(r compton.Registrar, videoId, videoTitle, videoDuration string) compton.Element {

	originLink := els.A(youtube_urls.VideoUrl(videoId).String())
	originLink.SetAttribute("target", "_top")

	linkColumn := flex_items.FlexItems(r, direction.Column).
		RowGap(size.Unset)

	if videoTitle == "" {
		videoTitle = "Watch at origin"
	}

	linkText := fspan.Text(r, videoTitle).
		FontWeight(font_weight.Bolder).
		ForegroundColor(color.Cyan)
	linkColumn.Append(linkText)

	if dur, err := strconv.ParseInt(videoDuration, 10, 64); err == nil {
		durationRow := flex_items.FlexItems(r, direction.Row).
			ColumnGap(size.Small).
			JustifyContent(align.Center).
			FontSize(size.Small)
		durationTitle := fspan.Text(r, "Duration:").ForegroundColor(color.Gray)
		durationValue := fspan.Text(r, formatSeconds(dur))
		durationRow.Append(durationTitle, durationValue)
		linkColumn.Append(durationRow)
	}

	originLink.Append(linkColumn)

	return originLink
}
