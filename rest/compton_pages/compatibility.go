package compton_pages

import (
	"encoding/json"
	"errors"
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
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"github.com/boggydigital/redux"
)

const (
	additionalInfoText = "The developer has provided additional information regarding %s support for this game. " +
		"Learn more on their Community Page. "
	additionalInfoLink = "View Developer Post"
)

const (
	steamDeck = "Steam Deck"
	steamOs   = "SteamOS"
)

var messageByCategory = map[string]string{
	"Verified": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<a class='Verified' target='_top' href='/search?steam-deck-app-compatibility-category=Verified'>Verified</a> on %s. " +
		"This game is fully functional on %s, and works great with the built-in controls and display.",
	"Playable": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<a class='Playable' target='_top' href='/search?steam-deck-app-compatibility-category=Playable'>Playable</a> on %s. " +
		"This game is functional on %s, but might require extra effort to interact with or configure.",
	"Unsupported": "Valve’s testing indicates that <span class='title'>%s</span> is " +
		"<a class='Unsupported' target='_top' href='/search?steam-deck-app-compatibility-category=Unsupported'>Unsupported</a> on %s. " +
		"Some or all of this game currently doesn't function on %s.",
	"Unknown": "Valve is still learning about <span class='title'>%s</span> on %s. " +
		"We do not currently have further information regarding %s compatibility.",
}

var displayTypeColors = map[string]color.Color{
	"Verified":    color.Green,
	"Playable":    color.Orange,
	"Unsupported": color.Red,
	"Unknown":     color.RepGray,
}

func Compatibility(id string, rdx redux.Readable) compton.PageElement {
	title, _ := rdx.GetLastVal(vangogh_integration.TitleProperty, id)

	s := compton_fragments.ProductSection(compton_data.CompatibilitySection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	compatibilityRow := compton.FlexItems(s, direction.Row).
		RowGap(size.Normal).
		ColumnGap(size.Large).
		ColumnWidthRule(size.XXXSmall)
	pageStack.Append(compatibilityRow)

	for _, rrp := range compton_fragments.ProductProperties(s, id, rdx, compton_data.CompatibilityProperties...) {
		compatibilityRow.Append(rrp)
	}

	deckCompatibilityReport, err := getDeckAppCompatibilityReport(id, rdx)
	if err != nil {
		errorFspan := compton.Fspan(s, err.Error())
		pageStack.Append(errorFspan)
		return s
	}

	if deckCompatibilityReport == nil {
		return s
	}

	for _, device := range []string{steamDeck, steamOs} {
		addSteamCompatibilitySection(s, pageStack, id, title, deckCompatibilityReport, device, rdx)
	}

	return s
}

func getDeckAppCompatibilityReport(gogId string, rdx redux.Readable) (*steam_integration.DeckAppCompatibilityReport, error) {

	var steamAppId string
	if sai, ok := rdx.GetLastVal(vangogh_integration.SteamAppIdProperty, gogId); ok && sai != "" {
		steamAppId = sai
	} else {
		return nil, errors.New("no steam app id for gog id " + gogId)
	}

	deckAppCompatibilityReportDir, err := vangogh_integration.AbsProductTypeDir(vangogh_integration.SteamDeckCompatibilityReport)
	if err != nil {
		return nil, err
	}

	kvDeckAppCompatibilityReport, err := kevlar.New(deckAppCompatibilityReportDir, kevlar.JsonExt)
	if err != nil {
		return nil, err
	}

	rcDeckAppCompatibilityReport, err := kvDeckAppCompatibilityReport.Get(steamAppId)
	if err != nil {
		return nil, err
	}
	defer rcDeckAppCompatibilityReport.Close()

	var deckCompatibilityReport steam_integration.DeckAppCompatibilityReport
	if err = json.NewDecoder(rcDeckAppCompatibilityReport).Decode(&deckCompatibilityReport); err != nil {
		return nil, err
	}

	return &deckCompatibilityReport, nil
}

func addSteamCompatibilitySection(r compton.Registrar, pageStack compton.Element, id, title string, dacr *steam_integration.DeckAppCompatibilityReport, steamDevice string, rdx redux.Readable) {

	pageStack.Append(compton.SectionDivider(r, compton.Text(steamDevice)))

	var steamAppCompatibilityProperty string
	switch steamDevice {
	case steamOs:
		steamAppCompatibilityProperty = vangogh_integration.SteamOsAppCompatibilityCategoryProperty
	case steamDeck:
		fallthrough
	default:
		steamAppCompatibilityProperty = vangogh_integration.SteamDeckAppCompatibilityCategoryProperty
	}

	if category, ok := rdx.GetLastVal(steamAppCompatibilityProperty, id); ok {
		message := fmt.Sprintf(messageByCategory[category], title, steamDevice, steamDevice)
		divMessage := compton.DivText(message)
		divMessage.AddClass("message")
		pageStack.Append(divMessage)
	}

	if blogUrl := dacr.GetBlogUrl(); blogUrl != "" {
		additionalInfo := compton.Span()
		additionalInfo.Append(compton.Text(fmt.Sprintf(additionalInfoText, steamDevice)))

		link := compton.A(blogUrl)
		link.Append(compton.Fspan(r, additionalInfoLink).
			FontWeight(font_weight.Bolder).
			ForegroundColor(color.Cyan))
		link.SetAttribute("target", "_top")
		additionalInfo.Append(link)

		pageStack.Append(additionalInfo)
	}

	var getResults func() []string
	switch steamDevice {
	case steamOs:
		getResults = dacr.GetSteamOsResults
	case steamDeck:
		fallthrough
	default:
		getResults = dacr.GetSteamDeckResults
	}

	results := getResults()

	if len(results) > 0 {
		pageStack.Append(compton.Hr())
	}

	var getDisplayTypes func() []string
	switch steamDevice {
	case steamOs:
		getDisplayTypes = dacr.GetSteamOsDisplayTypes
	case steamDeck:
		fallthrough
	default:
		getDisplayTypes = dacr.GetSteamDeckDisplayTypes
	}

	displayTypes := getDisplayTypes()

	var decodeToken func(string) string
	switch steamDevice {
	case steamOs:
		decodeToken = steam_integration.SteamOsDecodeLocToken
	case steamDeck:
		fallthrough
	default:
		decodeToken = steam_integration.SteamDeckDecodeLocToken
	}

	ul := compton.Ul()
	if len(displayTypes) == len(results) {
		for ii, result := range results {

			dt := displayTypes[ii]
			resultRow := compton.FlexItems(r, direction.Row).AlignItems(align.Center)
			resultRow.AddClass("nowrap")

			displayTypeIcon := compton.Fspan(r, "").ForegroundColor(displayTypeColors[dt])
			displayTypeIcon.AddClass("svg")
			displayTypeIcon.Append(compton.SvgUse(r, compton.Circle))
			decodedResult := decodeToken(result)
			if decodedResult == "" {
				decodedResult = result
			}
			displayTypeMessage := compton.Fspan(r, decodedResult).FontSize(size.Small)
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

}
