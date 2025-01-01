package compton_fragments

import (
	"fmt"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"golang.org/x/exp/maps"
	"golang.org/x/net/html/atom"
	"slices"
	"strings"
)

func SearchForm(r compton.Registrar, query map[string][]string, searchQuery *compton.FrowElement, rdx kevlar.ReadableRedux) compton.Element {

	form := compton.Form("/search", "GET")
	formStack := compton.FlexItems(r, direction.Column)
	form.Append(formStack)

	if searchQuery != nil {
		formStack.Append(compton.FICenter(r, searchQuery))
	}

	submitRow := compton.FlexItems(r, direction.Row).JustifyContent(align.Center)
	submit := compton.InputValue(r, input_types.Submit, "Submit Query")
	submitRow.Append(submit)
	formStack.Append(submitRow)

	inputsGrid := compton.GridItems(r).JustifyContent(align.Center).GridTemplateRows(size.XLarge)
	formStack.Append(inputsGrid)

	searchInputs(r, query, inputsGrid, rdx)

	// duplicating Submit button after inputs at the end
	formStack.Append(submitRow)

	return form
}

var binDatalist = map[string]string{
	"true":  "Yes",
	"false": "No",
}

var typesDigest = []vangogh_local_data.ProductType{
	vangogh_local_data.AccountProducts,
	vangogh_local_data.CatalogProducts,
}

func stringerDatalist[T fmt.Stringer](items []T) map[string]string {
	dl := make(map[string]string)
	for _, item := range items {
		str := item.String()
		dl[str] = compton_data.PropertyTitles[str]
	}
	return dl
}

func typesDatalist() map[string]string {
	return stringerDatalist(typesDigest)
}

func operatingSystemsDatalist() map[string]string {
	return stringerDatalist([]vangogh_local_data.OperatingSystem{
		vangogh_local_data.Windows,
		vangogh_local_data.MacOS,
		vangogh_local_data.Linux})
}

var sortProperties = []string{
	vangogh_local_data.GlobalReleaseDateProperty,
	vangogh_local_data.GOGReleaseDateProperty,
	vangogh_local_data.GOGOrderDateProperty,
	vangogh_local_data.TitleProperty,
	vangogh_local_data.RatingProperty,
	vangogh_local_data.DiscountPercentageProperty,
	vangogh_local_data.HLTBHoursToCompleteMainProperty,
	vangogh_local_data.HLTBHoursToCompletePlusProperty,
	vangogh_local_data.HLTBHoursToComplete100Property}

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
	return propertiesDatalist([]string{"GAME", "PACK", "DLC"})
}

func steamDeckDatalist() map[string]string {
	return propertiesDatalist([]string{"Verified", "Playable", "Unsupported", "Unknown"})
}

func languagesDatalist() map[string]string {
	dl := make(map[string]string)
	for _, lc := range maps.Keys(compton_data.LanguageTitles) {
		dl[lc] = compton_data.FormatLanguage(lc)
	}
	return dl
}

func tagsDatalist(rdx kevlar.ReadableRedux) map[string]string {
	dl := make(map[string]string)
	for _, tagId := range rdx.Keys(vangogh_local_data.TagNameProperty) {
		if tagName, ok := rdx.GetLastVal(vangogh_local_data.TagNameProperty, tagId); ok {
			dl[tagId] = tagName
		}
	}
	return dl
}

func propertyValuesDatalist(property string, rdx kevlar.ReadableRedux) map[string]string {
	dl := make(map[string]string)
	for _, id := range rdx.Keys(property) {
		if vals, ok := rdx.GetAllValues(property, id); ok {
			for _, val := range vals {
				dl[val] = val
			}
		}
	}
	return dl
}

func searchInputs(r compton.Registrar, query map[string][]string, container compton.Element, rdx kevlar.ReadableRedux) {
	for ii, property := range compton_data.SearchProperties {
		title := compton_data.PropertyTitles[property]
		value := strings.Join(query[property], ", ")
		titleInput := compton.TISearchValue(r, title, property, value)

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
			case vangogh_local_data.TypesProperty:
				datalist = typesDatalist()
			case vangogh_local_data.OperatingSystemsProperty:
				datalist = operatingSystemsDatalist()
			case vangogh_local_data.SortProperty:
				datalist = sortDatalist()
			case vangogh_local_data.ProductTypeProperty:
				datalist = productTypesDatalist()
			case vangogh_local_data.SteamDeckAppCompatibilityCategoryProperty:
				datalist = steamDeckDatalist()
			case vangogh_local_data.LanguageCodeProperty:
				datalist = languagesDatalist()
			case vangogh_local_data.TagIdProperty:
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
