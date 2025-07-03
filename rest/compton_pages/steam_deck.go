package compton_pages

import (
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/redux"
)

const (
	additionalInfoText = "The developer has provided additional information regarding Steam Deck support for this game. " +
		"Learn more on their Community Page. "
	additionalInfoLink = "View Developer Post"
)

var messageByCategory = map[string]string{
	"Verified": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<a class='category verified' target='_top' href='/search?steam-deck-app-compatibility-category=Verified'>Verified</a> on Steam Deck. " +
		"This game is fully functional on Steam Deck, and works great with the built-in controls and display.",
	"Playable": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<a class='category playable' target='_top' href='/search?steam-deck-app-compatibility-category=Playable'>Playable</a> on Steam Deck. " +
		"This game is functional on Steam Deck, but might require extra effort to interact with or configure.",
	"Unsupported": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<a class='category unsupported' target='_top' href='/search?steam-deck-app-compatibility-category=Unsupported'>Unsupported</a> on Steam Deck. " +
		"Some or all of this game currently doesn't function on Steam Deck.",
	"Unknown": "Valve is still learning about <span class='title'>%s</span>. " +
		"We do not currently have further information regarding Steam Deck compatibility.",
}

var displayTypeColors = map[string]color.Color{
	"Verified":    color.Green,
	"Playable":    color.Orange,
	"Unsupported": color.Red,
	"Unknown":     color.RepGray,
}

func SteamDeck(id string, dacr *steam_integration.DeckAppCompatibilityReport, rdx redux.Readable) compton.PageElement {
	title, _ := rdx.GetLastVal(vangogh_integration.TitleProperty, id)

	s := compton_fragments.ProductSection(compton_data.SteamDeckSection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if category, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {
		message := fmt.Sprintf(messageByCategory[category], title)
		divMessage := compton.DivText(message)
		divMessage.AddClass("message")
		pageStack.Append(divMessage)
	}

	if blogUrl := dacr.GetBlogUrl(); blogUrl != "" {
		additionalInfo := compton.Span()
		additionalInfo.Append(compton.Text(additionalInfoText))

		link := compton.A(blogUrl)
		link.Append(compton.Fspan(s, additionalInfoLink).
			FontWeight(font_weight.Bolder).
			ForegroundColor(color.Cyan))
		link.SetAttribute("target", "_top")
		additionalInfo.Append(link)

		pageStack.Append(additionalInfo)
	}

	results := dacr.GetResults()

	if len(results) > 0 {
		pageStack.Append(compton.Hr())
	}

	displayTypes := dacr.GetDisplayTypes()

	ul := compton.Ul()
	if len(displayTypes) == len(results) {
		for ii, result := range results {

			dt := displayTypes[ii]
			resultRow := compton.FlexItems(s, direction.Row).AlignItems(align.Center)
			resultRow.AddClass("nowrap")

			displayTypeIcon := compton.Fspan(s, "").ForegroundColor(displayTypeColors[dt])
			displayTypeIcon.AddClass("svg")
			displayTypeIcon.Append(compton.SvgUse(s, compton.Circle))
			decodedResult := steam_integration.DecodeLocToken(result)
			if decodedResult == "" {
				decodedResult = result
			}
			displayTypeMessage := compton.Fspan(s, decodedResult)
			if dt == "Unknown" {
				displayTypeMessage.ForegroundColor(color.RepGray)
			}
			resultRow.Append(displayTypeIcon, displayTypeMessage)

			li := compton.ListItem()
			li.Append(resultRow)

			ul.Append(li)

			if ii != len(results)-1 {
				ul.Append(compton.Hr())
			}

		}
	}
	pageStack.Append(ul)

	return s
}
