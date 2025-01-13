package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/kevlar"
)

func FormatLabels(id string, rdx kevlar.ReadableRedux) []compton.FormattedLabel {
	owned := false
	if op, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok {
		owned = op == vangogh_integration.TrueValue
	}

	fmtLabels := make([]compton.FormattedLabel, 0, len(compton_data.LabelProperties))

	for _, p := range compton_data.LabelProperties {
		fmtLabels = append(fmtLabels, formatLabel(id, p, owned, rdx))
	}

	return fmtLabels
}

func formatLabel(id, property string, owned bool, rdx kevlar.ReadableRedux) compton.FormattedLabel {

	fmtLabel := compton.FormattedLabel{
		Property: property,
	}

	fmtLabel.Title, _ = rdx.GetLastVal(property, id)
	switch property {
	case vangogh_integration.OwnedProperty:
		if pvr, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
			fmtLabel.Class = pvr
		}
		fallthrough
	case vangogh_integration.WishlistedProperty:
		fallthrough
	case vangogh_integration.PreOrderProperty:
		fallthrough
	case vangogh_integration.ComingSoonProperty:
		fallthrough
	case vangogh_integration.InDevelopmentProperty:
		fallthrough
	case vangogh_integration.IsFreeProperty:
		if fmtLabel.Title == "true" {
			fmtLabel.Title = compton_data.LabelTitles[property]
			break
		}
		fmtLabel.Title = ""
	case vangogh_integration.ProductTypeProperty:
		if fmtLabel.Title == "GAME" {
			fmtLabel.Title = ""
			break
		}
	case vangogh_integration.DiscountPercentageProperty:
		if owned {
			fmtLabel.Title = ""
			break
		}
		if fmtLabel.Title != "" && fmtLabel.Title != "0" {
			fmtLabel.Title = fmt.Sprintf("-%s%%", fmtLabel.Title)
			break
		}
		fmtLabel.Title = ""
	case vangogh_integration.TagIdProperty:
		if tagName, ok := rdx.GetLastVal(vangogh_integration.TagNameProperty, fmtLabel.Title); ok {
			fmtLabel.Title = tagName
			break
		}
	case vangogh_integration.DehydratedImageProperty:
		fallthrough
	case vangogh_integration.DehydratedVerticalImageProperty:
		fmtLabel.Title = property
	case vangogh_integration.StoreTagsProperty:
		if rdx.HasValue(vangogh_integration.StoreTagsProperty, id, "Good Old Game") {
			fmtLabel.Title = "GOG"
			fmtLabel.Class = "good-old-game"
		} else {
			fmtLabel.Title = ""
		}
	}
	return fmtLabel
}
