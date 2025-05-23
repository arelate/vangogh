package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
	"strings"
)

var ValidationResultsColors = map[vangogh_integration.ValidationResult]color.Color{
	vangogh_integration.ValidationResultUnknown:      color.Gray,
	vangogh_integration.ValidatedSuccessfully:        color.Green,
	vangogh_integration.ValidatedUnresolvedManualUrl: color.Teal,
	vangogh_integration.ValidatedMissingLocalFile:    color.Teal,
	vangogh_integration.ValidatedMissingChecksum:     color.Teal,
	vangogh_integration.ValidationError:              color.Orange,
	vangogh_integration.ValidatedChecksumMismatch:    color.Red,
}

func FormatBadges(id string, rdx redux.Readable) []compton.FormattedBadge {
	owned := false
	if lp, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok {
		owned = lp == vangogh_integration.TrueValue
	}

	fmtBadges := make([]compton.FormattedBadge, 0, len(compton_data.BadgeProperties))

	for _, p := range compton_data.BadgeProperties {
		if fmtBadge := formatBadge(id, p, owned, rdx); fmtBadge.Title != "" {
			fmtBadges = append(fmtBadges, fmtBadge)
		}
	}

	return fmtBadges
}

func formatBadge(id, property string, owned bool, rdx redux.Readable) compton.FormattedBadge {

	fmtBadge := compton.FormattedBadge{
		Class: property,
	}

	switch property {
	case vangogh_integration.OwnedProperty:
		if owned {
			fmtBadge.Title = "OWN"
			if pvr, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
				validationResult := vangogh_integration.ParseValidationResult(pvr)
				fmtBadge.Color = ValidationResultsColors[validationResult]
			} else {
				fmtBadge.Color = color.Gray
			}
		}
	case vangogh_integration.UserWishlistProperty:
		if wish, ok := rdx.GetLastVal(vangogh_integration.UserWishlistProperty, id); ok && wish == vangogh_integration.TrueValue {
			fmtBadge.Title = "WISH"
			fmtBadge.Color = color.Orange
		}
	case vangogh_integration.PreOrderProperty:
		if po, ok := rdx.GetLastVal(vangogh_integration.PreOrderProperty, id); ok && po == vangogh_integration.TrueValue {
			fmtBadge.Title = "PO"
			fmtBadge.Color = color.Teal
		}
	case vangogh_integration.InDevelopmentProperty:
		if inDev, ok := rdx.GetLastVal(vangogh_integration.InDevelopmentProperty, id); ok && inDev == vangogh_integration.TrueValue {
			fmtBadge.Title = "IN DEV"
			fmtBadge.Color = color.Teal
		}
	case vangogh_integration.IsDemoProperty:
		if demo, ok := rdx.GetLastVal(vangogh_integration.IsDemoProperty, id); ok && demo == vangogh_integration.TrueValue {
			fmtBadge.Title = "DEMO"
			fmtBadge.Color = color.Mint
		}
	case vangogh_integration.IsFreeProperty:
		if demo, ok := rdx.GetLastVal(vangogh_integration.IsDemoProperty, id); ok && demo == vangogh_integration.TrueValue {
			fmtBadge.Title = ""
		} else if free, ok := rdx.GetLastVal(vangogh_integration.IsFreeProperty, id); ok && free == vangogh_integration.TrueValue {
			fmtBadge.Title = "FREE"
			fmtBadge.Color = color.Mint
		}
	case vangogh_integration.ComingSoonProperty:
		if owned {
			fmtBadge.Title = ""
		} else if soon, ok := rdx.GetLastVal(vangogh_integration.ComingSoonProperty, id); ok && soon == vangogh_integration.TrueValue {
			fmtBadge.Title = "SOON"
			fmtBadge.Color = color.Teal
		}
	case vangogh_integration.ProductTypeProperty:
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok && pt != "GAME" {
			fmtBadge.Title = pt
			fmtBadge.Color = color.Foreground
		}
	case vangogh_integration.DiscountPercentageProperty:
		if owned {
			fmtBadge.Title = ""
		} else if dp, ok := rdx.GetLastVal(vangogh_integration.DiscountPercentageProperty, id); ok && dp != "0" {
			fmtBadge.Title = fmt.Sprintf("-%s%%", dp)
			fmtBadge.Color = color.Mint
		} else {
			fmtBadge.Title = ""
		}
	case vangogh_integration.TagIdProperty:
		if tagId, ok := rdx.GetLastVal(vangogh_integration.TagIdProperty, id); ok {
			if tagName, sure := rdx.GetLastVal(vangogh_integration.TagNameProperty, tagId); sure {
				fmtBadge.Title = tagName
				fmtBadge.Color = color.Indigo
			}
		}
	case vangogh_integration.LocalTagsProperty:
		if localTags, ok := rdx.GetAllValues(vangogh_integration.LocalTagsProperty, id); ok {
			fmtBadge.Title = strings.Join(localTags, ", ")
			fmtBadge.Color = color.Indigo
		}
	case vangogh_integration.StoreTagsProperty:
		if rdx.HasValue(vangogh_integration.StoreTagsProperty, id, "Good Old Game") {
			fmtBadge.Title = "GOG"
			fmtBadge.Color = color.Purple
			fmtBadge.Class = "good-old-game"
		} else {
			fmtBadge.Title = ""
		}
	}
	return fmtBadge
}
