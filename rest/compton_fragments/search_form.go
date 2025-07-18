package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"golang.org/x/net/html/atom"
	"maps"
	"slices"
	"strings"
)

func SearchForm(r compton.Registrar, query map[string][]string, searchQuery *compton.FrowElement, rdx redux.Readable) compton.Element {

	form := compton.Form("/search", "GET")
	formStack := compton.FlexItems(r, direction.Column)
	form.Append(formStack)

	if searchQuery != nil {
		formStack.Append(compton.FICenter(r, searchQuery))
	}

	submitRow := compton.FlexItems(r, direction.Row).JustifyContent(align.Center)

	submitNavLink := compton.NavLinks(r)
	submitNavLink.AppendSubmitLink(r, &compton.NavTarget{
		Href:  "#",
		Title: "Submit Query",
	})

	submitRow.Append(submitNavLink)
	formStack.Append(submitRow)

	inputs := compton.FlexItems(r, direction.Row).JustifyContent(align.Center).RowGap(size.Normal).Width(size.FullWidth)
	formStack.Append(inputs)

	searchInputs(r, query, inputs, rdx)

	// duplicating Submit button after inputs at the end
	formStack.Append(submitRow)

	return form
}

var binDatalist = map[string]string{
	"true":  "Yes",
	"false": "No",
}

func stringerDatalist[T fmt.Stringer](items []T) map[string]string {
	dl := make(map[string]string)
	for _, item := range items {
		str := item.String()
		dl[str] = compton_data.PropertyTitles[str]
	}
	return dl
}

func operatingSystemsDatalist() map[string]string {
	return stringerDatalist([]vangogh_integration.OperatingSystem{
		vangogh_integration.Windows,
		vangogh_integration.MacOS,
		vangogh_integration.Linux})
}

var sortProperties = []string{
	vangogh_integration.GlobalReleaseDateProperty,
	vangogh_integration.GOGReleaseDateProperty,
	vangogh_integration.GOGOrderDateProperty,
	vangogh_integration.SummaryRatingProperty,
	vangogh_integration.TitleProperty,
	vangogh_integration.RatingProperty,
	vangogh_integration.DiscountPercentageProperty,
	vangogh_integration.HltbHoursToCompleteMainProperty,
	vangogh_integration.HltbHoursToCompletePlusProperty,
	vangogh_integration.HltbHoursToComplete100Property}

func propertiesDatalist(properties []string) map[string]string {
	dl := make(map[string]string)
	for _, p := range properties {
		dl[p] = compton_data.PropertyTitles[p]
	}
	return dl
}

func sortDatalist() map[string]string {
	return propertiesDatalist(sortProperties)
}

func productTypesDatalist() map[string]string {
	return propertiesDatalist([]string{vangogh_integration.GameProductType,
		vangogh_integration.DlcProductType,
		vangogh_integration.PackProductType})
}

func steamDeckDatalist() map[string]string {
	return propertiesDatalist([]string{"Verified", "Playable", "Unsupported", "Unknown"})
}

func languagesDatalist() map[string]string {
	dl := make(map[string]string)
	for lc := range maps.Keys(compton_data.LanguageTitles) {
		dl[lc] = compton_data.FormatLanguage(lc)
	}
	return dl
}

func tagsDatalist(rdx redux.Readable) map[string]string {
	dl := make(map[string]string)
	for tagId := range rdx.Keys(vangogh_integration.TagNameProperty) {
		if tagName, ok := rdx.GetLastVal(vangogh_integration.TagNameProperty, tagId); ok {
			dl[tagId] = tagName
		}
	}
	return dl
}

func propertyValuesDatalist(property string, rdx redux.Readable) map[string]string {
	dl := make(map[string]string)
	for id := range rdx.Keys(property) {
		if vals, ok := rdx.GetAllValues(property, id); ok {
			for _, val := range vals {
				dl[val] = val
			}
		}
	}
	return dl
}

func searchInputs(r compton.Registrar, query map[string][]string, container compton.Element, rdx redux.Readable) {
	for ii, property := range compton_data.SearchProperties {
		title := compton_data.PropertyTitles[property]
		value := strings.Join(query[property], ", ")
		titleInput := compton.TISearchValue(r, title, property, value)
		titleInput.RowGap(size.XSmall)

		if ii == 0 {
			if input := titleInput.GetFirstElementByTagName(atom.Input); input != nil {
				input.SetAttribute("autofocus", "")
			}
		}

		var datalist map[string]string
		var listId string

		if slices.Contains(compton_data.BinaryDigestProperties, property) {
			datalist = binDatalist
			listId = "bin-list"
		} else if slices.Contains(compton_data.DigestProperties, property) {
			switch property {
			case vangogh_integration.OperatingSystemsProperty:
				datalist = operatingSystemsDatalist()
			case vangogh_integration.SortProperty:
				datalist = sortDatalist()
			case vangogh_integration.ProductTypeProperty:
				datalist = productTypesDatalist()
			case vangogh_integration.SteamOsAppCompatibilityCategoryProperty:
				fallthrough
			case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
				datalist = steamDeckDatalist()
			case vangogh_integration.LanguageCodeProperty:
				datalist = languagesDatalist()
			case vangogh_integration.TagIdProperty:
				datalist = tagsDatalist(rdx)
			default:
				datalist = propertyValuesDatalist(property, rdx)
			}
		}

		if len(datalist) > 0 {
			titleInput.SetDatalist(datalist, listId)
		}

		container.Append(titleInput)
	}
}
