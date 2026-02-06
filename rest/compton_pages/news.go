package compton_pages

import (
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

func News(gogId string, rdx redux.Readable, san *steam_integration.AppNews, all bool) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.NewsSection, gogId, rdx)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if rdx.HasKey(vangogh_integration.ChangelogProperty, gogId) {
		changelogLink := compton.A("/changelog?id=" + gogId)
		changelogLink.Append(compton.Fspan(s, "View GOG.com Changelog").
			FontSize(size.Small).
			ForegroundColor(color.RepForeground))
		changelogLink.SetAttribute("target", "_top")
		changelogLink.AddClass("internal")

		fmtNewsBadge := &compton.FormattedBadge{
			Title: "Steam News",
			Icon:  compton.NewsBroadcast,
			Color: color.RepForeground,
		}

		steamNewsSection := compton.SectionDivider(s, fmtNewsBadge)

		pageStack.Append(compton.FICenter(s, changelogLink))

		if san != nil {
			pageStack.Append(steamNewsSection)
		}
	}

	if san == nil {
		return s
	}

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
		href := "/news?id=" + gogId + "&all"
		if all {
			title = "Show only community announcements"
			href = "/news?id=" + gogId
		}

		communityAnnouncementsNavLink := compton.NavLinks(s)
		communityAnnouncementsNavLink.AppendLink(s, &compton.NavTarget{
			Href:  href,
			Title: title,
		})

		pageStack.Append(communityAnnouncementsNavLink)
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
			ForegroundColor(color.RepGray).
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
