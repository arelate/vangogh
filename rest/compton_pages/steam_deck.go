package compton_pages

import (
	"fmt"
	"github.com/arelate/southern_light/steam_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/elements/els"
	"github.com/boggydigital/compton/elements/flex_items"
	"github.com/boggydigital/compton/elements/fspan"
	"github.com/boggydigital/compton/elements/svg_use"
	"github.com/boggydigital/kevlar"
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

func SteamDeck(id string, dacr *steam_integration.DeckAppCompatibilityReport, rdx kevlar.ReadableRedux) compton.Element {
	title, _ := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)

	s := compton_fragments.ProductSection(compton_data.SteamDeckSection)

	pageStack := flex_items.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if category, ok := rdx.GetLastVal(vangogh_local_data.SteamDeckAppCompatibilityCategoryProperty, id); ok {
		message := fmt.Sprintf(messageByCategory[category], title)
		divMessage := els.DivText(message)
		divMessage.AddClass("message")
		pageStack.Append(divMessage)
	}

	results := dacr.GetResults()

	if len(results) > 0 {
		pageStack.Append(els.Hr())
	}

	if blogUrl := dacr.GetBlogUrl(); blogUrl != "" {
		pageStack.Append(els.AText("Read more in the Steam blog", blogUrl))
	}

	displayTypes := dacr.GetDisplayTypes()

	ul := els.Ul()
	if len(displayTypes) == len(results) {
		for ii, result := range results {

			dt := displayTypes[ii]
			resultRow := flex_items.FlexItems(s, direction.Row).AlignItems(align.Center)
			resultRow.AddClass("nowrap")

			displayTypeIcon := fspan.Text(s, "").ForegroundColor(displayTypeColors[dt])
			displayTypeIcon.AddClass("svg")
			displayTypeIcon.Append(svg_use.SvgUse(s, svg_use.Circle))
			decodedResult := steam_integration.DecodeLocToken(result)
			if decodedResult == "" {
				decodedResult = result
			}
			displayTypeMessage := fspan.Text(s, decodedResult)
			if dt == "Unknown" {
				displayTypeMessage.ForegroundColor(color.Gray)
			}
			resultRow.Append(displayTypeIcon, displayTypeMessage)

			li := els.ListItem()
			li.Append(resultRow)

			ul.Append(li)

			if ii != len(results)-1 {
				ul.Append(els.Hr())
			}

		}
	}
	pageStack.Append(ul)

	return s
}
