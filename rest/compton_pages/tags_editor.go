package compton_pages

import (
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh/rest/compton_styles"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/input_types"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"golang.org/x/exp/maps"
	"net/http"
	"slices"
	"strconv"
)

func TagsEditor(
	id string,
	owned bool,
	tagsProperty string,
	allValues map[string]string,
	selected map[string]any,
	rdx kevlar.ReadableRedux) compton.PageElement {

	tagsPropertyTitle := compton_data.PropertyTitles[tagsProperty]

	p, pageStack := compton_fragments.AppPage("Edit " + tagsPropertyTitle)
	p.RegisterStyles(compton_styles.VangoghStyles, "style/tag-editors.css")

	/* App navigation */

	appNavLinks := compton_fragments.AppNavLinks(p, "")

	pageStack.Append(compton.FICenter(p, appNavLinks))

	/* Product poster */

	if poster := compton_fragments.ProductPoster(p, id, rdx); poster != nil {
		pageStack.Append(poster)
	}

	/* Product title */

	productTitle, _ := rdx.GetLastVal(vangogh_local_data.TitleProperty, id)
	productHeading := compton.HeadingText(productTitle, 1)
	pageStack.Append(compton.FICenter(p, productHeading))

	/* Ownership notice */

	if !owned {
		ownershipNotice := compton.Fspan(p, "Tags modifications require product ownership").
			ForegroundColor(color.Yellow).FontWeight(font_weight.Bolder)

		pageStack.Append(compton.FICenter(p, ownershipNotice))
	}

	/* Tags Property Title */

	tagsPropertyHeading := compton_fragments.DetailsSummaryTitle(p, tagsPropertyTitle)

	dsTags := compton.DSLarge(p, tagsPropertyHeading, true).
		BackgroundColor(color.Highlight).
		ForegroundColor(color.Foreground).
		SummaryMarginBlockEnd(size.Normal).
		DetailsMarginBlockEnd(size.Unset)

	pageStack.Append(dsTags)

	/* Tag Values Switches */

	action := ""
	switch tagsProperty {
	case vangogh_local_data.LocalTagsProperty:
		action = "/local-tags/apply"
	case vangogh_local_data.TagIdProperty:
		action = "/tags/apply"
	default:
		panic("unknown tags property editor")
	}

	editTagsForm := compton.Form(action, http.MethodGet)
	swColumn := compton.FlexItems(p, direction.Column).AlignContent(align.Center)

	idInput := compton.InputValue(p, input_types.Hidden, id)
	idInput.SetName(vangogh_local_data.IdProperty)
	swColumn.Append(idInput)

	conditionInput := compton.InputValue(p, input_types.Hidden, strconv.FormatBool(owned))
	conditionInput.SetName("condition")
	swColumn.Append(conditionInput)

	keys := maps.Keys(allValues)
	slices.Sort(keys)

	for _, vid := range keys {
		label := allValues[vid]
		_, has := selected[vid]
		swColumn.Append(switchLabel(p, vid, label, has, !owned))
	}

	newValueInput := compton.Input(p, input_types.Text)
	newValueInput.SetName("new-property-value")
	newValueInput.SetPlaceholder("Add new value")
	swColumn.Append(newValueInput)

	applyButton := compton.InputValue(p, input_types.Submit, "Apply")
	swColumn.Append(applyButton)

	editTagsForm.Append(swColumn)
	dsTags.Append(editTagsForm)

	/* Footer */

	pageStack.Append(compton.Br(), compton_fragments.Footer(p))

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
