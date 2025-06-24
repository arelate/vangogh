package compton_fragments

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
)

func SteamNewsItem(r compton.Registrar, item steam_integration.NewsItem, open bool) compton.Element {

	container := compton.FlexItems(r, direction.Column).RowGap(size.Normal)

	newItemHeading := compton.HeadingText(item.Title, 3)

	container.Append(newItemHeading)

	title := "News"
	if len(item.Tags) > 0 {
		for _, nit := range item.Tags {
			if tagName, ok := compton_data.SteamNewsTags[nit]; ok {
				title = tagName
				break
			}
		}
	}

	fr := compton.Frow(r).
		FontSize(size.XSmall)
	fr.Heading(title)
	fr.PropVal("Posted", EpochDate(item.Date))

	if item.Author != "" {
		fr.PropVal("Author", item.Author)
	}
	if item.FeedType != compton_data.FeedTypeCommunityAnnouncement {
		// only showing feed label that are not community announcements
		fr.PropVal("Feed", item.FeedLabel)
	}

	fr.LinkColor("Source", item.Url, color.Cyan)

	container.Append(fr)

	ds := compton.DSSmall(r, "News item", open)

	itemContents := compton.PreText(item.Contents)

	itemContents.AddClass("steam-news-item")

	ds.Append(itemContents)

	container.Append(ds)

	return container
}
