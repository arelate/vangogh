package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
)

func SteamNews(id string, san *steam_integration.AppNews, all bool) compton.PageElement {
	s := compton_fragments.ProductSection(compton_data.SteamNewsSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	communityAnnouncements := make([]steam_integration.NewsItem, 0, len(san.NewsItems))
	for _, ni := range san.NewsItems {
		if ni.FeedType != compton_data.FeedTypeCommunityAnnouncement {
			continue
		}
		communityAnnouncements = append(communityAnnouncements, ni)
	}

	if len(san.NewsItems) > 0 &&
		len(communityAnnouncements) < len(san.NewsItems) {
		title := "Show all news items types"
		href := "/steam-news?id=" + id + "&all"
		if all {
			title = "Show only community announcements"
			href = "/steam-news?id=" + id
		}
		pageStack.Append(compton_fragments.Button(s, title, href))
	}

	newsItems := communityAnnouncements
	if all {
		newsItems = san.NewsItems
	}

	if len(newsItems) == 0 {
		title := "Community announcements are not available for this product"
		if all {
			title = "Steam news are not available for this product"
		}
		fs := compton.Fspan(s, title).
			ForegroundColor(color.Gray).
			TextAlign(align.Center)
		pageStack.Append(compton.FICenter(s, fs))
	}

	for ii, ni := range newsItems {
		if srf := compton_fragments.SteamNewsItem(s, ni, ii == 0); srf != nil {
			pageStack.Append(srf)
		}
		if ii < len(newsItems)-1 {
			pageStack.Append(compton.Hr())
		}
	}

	return s
}
