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
	"github.com/boggydigital/redux"
)

var messageByCategory = map[string]string{
	"Verified": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<span class='category verified'>Verified</span> on Steam Deck. " +
		"This game is fully functional on Steam Deck, and works great with the built-in controls and display.",
	"Playable": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<span class='category playable'>Playable</span> on Steam Deck. " +
		"This game is functional on Steam Deck, but might require extra effort to interact with or configure.",
	"Unsupported": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<span class='category unsupported'>Unsupported</span> on Steam Deck. " +
		"Some or all of this game currently doesn't function on Steam Deck.",
	"Unknown": "Valve is still learning about <span class='title'>%s</span>. " +
		"We do not currently have further information regarding Steam Deck compatibility.",
}

var displayTypeColors = map[string]color.Color{
	"Verified":    color.Green,
	"Playable":    color.Orange,
	"Unsupported": color.Red,
	"Unknown":     color.Gray,
}

func SteamDeck(id string, dacr *steam_integration.DeckAppCompatibilityReport, rdx redux.Readable) compton.PageElement {
	title, _ := rdx.GetLastVal(vangogh_integration.TitleProperty, id)

	s := compton_fragments.ProductSection(compton_data.SteamDeckSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if category, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok {
		message := fmt.Sprintf(messageByCategory[category], title)
		divMessage := compton.DivText(message)
		divMessage.AddClass("message")
		pageStack.Append(divMessage)
	}

	results := dacr.GetResults()

	if len(results) > 0 {
		pageStack.Append(compton.Hr())
	}

	if blogUrl := dacr.GetBlogUrl(); blogUrl != "" {
		pageStack.Append(compton.AText("Read more in the Steam blog", blogUrl))
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
				displayTypeMessage.ForegroundColor(color.Gray)
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
