package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/compton/elements/details_summary"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
	"github.com/boggydigital/yet_urls/youtube_urls"
	"regexp"
	"strings"
)

func SteamNewsItem(r compton.Registrar, item steam_integration.NewsItem, open bool) compton.Element {

	container := flex_items.FlexItems(r, direction.Column).RowGap(size.Normal)

	newItemHeading := els.HeadingText(item.Title, 3)

	container.Append(newItemHeading)

	headingTitle := "News"
	if len(item.Tags) > 0 {
		for _, nit := range item.Tags {
			if tagName, ok := compton_data.SteamNewsTags[nit]; ok {
				headingTitle = tagName
				break
			}
		}
	}

	header := SteamReviewHeadingRow(r, headingTitle)

	AppendSteamReviewPropertyValue(r, header, "Posted:", EpochDate(item.Date))
	if item.Author != "" {
		AppendSteamReviewPropertyValue(r, header, "Author:", item.Author)
	}
	if item.FeedType != compton_data.FeedTypeCommunityAnnouncement {
		// only showing feed label that are not community announcements
		AppendSteamReviewPropertyValue(r, header, "Feed:", item.FeedLabel)
	}

	sourceLink := SteamNewsLink(r, "Source", item.Url)
	header.Append(sourceLink)

	container.Append(header)

	dsTitle := fspan.Text(r, "Show news item").
		FontWeight(font_weight.Bolder).
		ForegroundColor(color.Gray)

	ds := details_summary.Smaller(r, dsTitle, open)

	itemContents := els.DivText(steamNewsToHTML(item.Contents))
	itemContents.AddClass("steam-news-item")

	ds.Append(itemContents)

	container.Append(ds)

	return container
}

func SteamNewsLink(r compton.Registrar, title, href string) compton.Element {
	linkDecoration := fspan.Text(r, "").
		FontWeight(font_weight.Bolder).
		ForegroundColor(color.Cyan)
	link := els.AText(title, href)
	link.SetAttribute("target", "_top")
	linkDecoration.Append(link)
	return linkDecoration
}

var preFormatters = map[string]string{
	"\n\n": "\n",
}

var formatters = map[string]string{
	//"[<a":                   "<a",
	//"/a>]":                  "/a>",
	"[b]":                "<b>",
	"[/b]":               "</b>",
	"[B]":                "<b>",
	"[/B]":               "</b>",
	"[<em>":              "<em>",
	"</em>]":             "</em>",
	"[u]":                "<u>",
	"[/u]":               "</u>",
	"[i]":                "<i>",
	"[/i]":               "</i>",
	"[I]":                "<i>",
	"[/I]":               "</i>",
	"[strike]":           "<s>",
	"[/strike]":          "</s>",
	"[h1]":               "<h1>",
	"[/h1]":              "</h1>",
	"[H1]":               "<h1>",
	"[/H1]":              "</h1>",
	"[h2]":               "<h2>",
	"[/h2]":              "</h2>",
	"[H2]":               "<h2>",
	"[/H2]":              "</h2>",
	"[h3]":               "<h3>",
	"[/h3]":              "</h3>",
	"[H3]":               "<h3>",
	"[/H3]":              "</h3>",
	"[list]":             "<ul>",
	"[List]":             "<ul>",
	"[*]":                "<li>",
	"[/*]":               "</li>",
	"[/list]":            "</ul>",
	"[/List]":            "</ul>",
	"[olist]":            "<ol>",
	"[/olist]":           "</ol>",
	"\n":                 "<br>",
	"{STEAM_CLAN_IMAGE}": "https://clan.akamai.steamstatic.com/images/",
	//"{STEAM_CLAN_IMAGE}": "https://cdn.cloudflare.steamstatic.com/steamcommunity/public/images/clans/",
	"[img]":  "<img src='",
	"[/img]": "'/>",
	//"[img=":                 "[",
	//"[IMG]":                 "<img src='",
	//"[/IMG]":                "'/>",
	"[hr][/hr]":  "<hr/>",
	"[spoiler]":  "<mark class='spoiler' tabindex='0' title='press and hold to reveal'>",
	"[/spoiler]": "</mark>",
	"[quote]":    "&quot;",
	"[/quote]":   "&quot;",
	"[code]":     "<pre>",
	"[/code]":    "</pre>",
	//"[url]":      "<a href='",
	//"[url=":                 "",
	"[and]": "&",
	//"[/url]":                "'/>",
	"[table]":               "<table>",
	"[/table]":              "</table>",
	"[at]":                  "@",
	"[dot]":                 ".",
	"[redacted]":            "<mark class='spoiler' tabindex='0' title='press and hold to reveal'>",
	"[/redacted]":           "</mark>",
	"[cms-block]":           "",
	"[tr]":                  "<tr>",
	"[/tr]":                 "</tr>",
	"[td]":                  "<td>",
	"[/td]":                 "</td>",
	"[th]":                  "<th>",
	"[/th]":                 "</th>",
	"[expand type=details]": "<details><summary>(Tap to expand)</summary>",
	"[/expand]":             "</details>",
}

func steamNewsToHTML(c string) string {

	for t := range preFormatters {
		c = strings.Replace(c, t, preFormatters[t], -1)
	}

	for t := range formatters {
		c = strings.Replace(c, t, formatters[t], -1)
	}

	c = replaceUrls(c)
	c = replaceYoutubePreviews(c)

	return c
}

var (
	steamNewsItemUrl    = regexp.MustCompile(`(?s)\[url=.*?\[/url]`)
	steamYoutubePreview = regexp.MustCompile(`(?s)\[previewyoutube=.*?\[/previewyoutube]`)
)

func extractUrls(c string) []string {
	return steamNewsItemUrl.FindAllString(c, -1)
}

func extractYoutubePreviews(c string) []string {
	return steamYoutubePreview.FindAllString(c, -1)
}

func replaceUrls(c string) string {
	urls := extractUrls(c)
	for _, u := range urls {
		nu := strings.Replace(u, "[url=", "<a target='_top' href='", 1)
		nu = strings.Replace(nu, "]", "'>", 1)
		nu = strings.Replace(nu, "[/url]", "</a>", 1)
		c = strings.Replace(c, u, nu, 1)
	}
	return c
}

func replaceYoutubePreviews(c string) string {
	youtubePreviews := extractYoutubePreviews(c)
	for _, yp := range youtubePreviews {
		vidId := strings.TrimPrefix(yp, "[previewyoutube=")
		vidId = strings.TrimSuffix(vidId, "][/previewyoutube]")
		vidId = strings.Split(vidId, ";")[0]

		nyp := fmt.Sprintf("<p><a target='_top' href='%s'>YouTube Link</a></p>", youtube_urls.VideoUrl(vidId))

		c = strings.Replace(c, yp, nyp, -1)
	}
	return c
}
