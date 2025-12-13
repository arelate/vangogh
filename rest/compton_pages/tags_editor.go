package compton_pages

import (
	"maps"
	"net/http"
	"slices"
	"strconv"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/issa"
	"github.com/boggydigital/redux"
)

func TagsEditor(
	id string,
	owned bool,
	tagsProperty string,
	allValues map[string]string,
	selected map[string]any,
	rdx redux.Readable) compton.PageElement {

	tagsPropertyTitle := compton_data.PropertyTitles[tagsProperty]

	p, pageStack := compton_fragments.AppPage("Edit " + tagsPropertyTitle)
	p.RegisterStyles(compton_styles.Styles, "product.css")
	p.RegisterStyles(compton_styles.Styles, "tag-editors.css")

	// tinting document background color to the representative product color
	if imageId, ok := rdx.GetLastVal(vangogh_integration.ImageProperty, id); ok && imageId != "" {
		if repColor, sure := rdx.GetLastVal(vangogh_integration.RepColorProperty, imageId); sure && repColor != issa.NeutralRepColor {
			p.SetAttribute("style", "--c-rep:"+repColor)
		}
	}

	var title string
	if tp, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, id); ok {
		title = tp
	}

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")
	pageStack.Append(compton.FICenter(p, appNavLinks))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(compton.FICenter(p, poster))
	}

	/* Product title */

	productHeading := compton.Heading(2)
	productHeading.Append(compton.Fspan(p, title).TextAlign(align.Center))
	pageStack.Append(compton.FICenter(p, productHeading))

	/* Ownership notice */

	if !owned {
		ownershipNotice := compton.Fspan(p, "Tags modifications require product ownership").
			ForegroundColor(color.Yellow).FontWeight(font_weight.Bolder)

		pageStack.Append(compton.FICenter(p, ownershipNotice))
	}

	/* Tags Property Title */

	dsTags := compton.DSLarge(p, tagsPropertyTitle, true).
		BackgroundColor(color.Highlight).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset)

	pageStack.Append(dsTags)

	/* Tag Values Switches */

	action := ""
	switch tagsProperty {
	case vangogh_integration.LocalTagsProperty:
		action = "/local-tags/apply"
	case vangogh_integration.TagIdProperty:
		action = "/tags/apply"
	default:
		panic("unknown tags property editor")
	}

	editTagsForm := compton.Form(action, http.MethodGet)
	swColumn := compton.FlexItems(p, direction.Column).
		AlignContent(align.Center).
		JustifyContent(align.Start).
		Width(size.XXXLarge)

	idInput := compton.InputValue(p, input_types.Hidden, id)
	idInput.SetName(vangogh_integration.IdProperty)
	swColumn.Append(idInput)

	conditionInput := compton.InputValue(p, input_types.Hidden, strconv.FormatBool(owned))
	conditionInput.SetName("condition")
	swColumn.Append(conditionInput)

	keys := maps.Keys(allValues)
	sortedKeys := slices.Sorted(keys)

	for _, vid := range sortedKeys {
		label := allValues[vid]
		_, has := selected[vid]
		swColumn.Append(switchLabel(p, vid, label, has, !owned))
	}

	newValueInput := compton.Input(p, input_types.Text)
	newValueInput.SetName("new-property-value")
	newValueInput.SetPlaceholder("Add new value")

	swColumn.Append(newValueInput)

	applyNavLink := compton.NavLinks(p)
	applyNavLink.AppendSubmitLink(p, &compton.NavTarget{
		Href:  "#",
		Title: "Apply",
	})
	swColumn.Append(applyNavLink)

	centerColumn := compton.FlexItems(p, direction.Column).AlignItems(align.Center)
	centerColumn.Append(swColumn)

	editTagsForm.Append(centerColumn)
	dsTags.Append(editTagsForm)

	/* Footer */

	pageStack.Append(compton.Br(), compton.FICenter(p, compton_fragments.GitHubLink(p), compton_fragments.LogoutLink(p)))

	return p
}

func switchLabel(r compton.Registrar, id, label string, checked, disabled bool) compton.Element {
	row := compton.FlexItems(r, direction.Row).AlignItems(align.Center)

	switchElement := compton.Switch(r)
	switchElement.SetId(id)
	switchElement.SetValue(id)
	switchElement.SetChecked(checked)
	switchElement.SetDisabled(disabled)
	switchElement.SetName("value") //using the same name for all binary properties

	labelElement := compton.Label(id)
	labelElement.Append(compton.Text(label))

	row.Append(switchElement, labelElement)

	return row
}
