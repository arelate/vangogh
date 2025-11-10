package compton_fragments

import (
	"strings"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
)

func FormatBadges(id string, rdx redux.Readable, badgeProperties []string, permissions ...author.Permission) []compton.FormattedBadge {

	fmtBadges := make([]compton.FormattedBadge, 0, len(badgeProperties))

	ppo := compton_data.PermittedProperties(badgeProperties, permissions...)

	for p := range ppo {
		if fmtBadge := formatBadge(id, p, rdx); fmtBadge.Title != "" || fmtBadge.Icon != compton.NoSymbol {
			fmtBadges = append(fmtBadges, fmtBadge)
		}
	}

	return fmtBadges
}

func formatBadge(id, property string, rdx redux.Readable) compton.FormattedBadge {

	fmtBadge := compton.FormattedBadge{
		Color: color.RepGray,
	}

	productDvs := vangogh_integration.NewProductDvs(id, rdx)
	productDownloadStatus := productDvs.DownloadStatus()
	productValidationStatus := productDvs.ValidationStatus()

	owned := false
	if lp, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok {
		owned = lp == vangogh_integration.TrueValue
	}

	var productType string
	if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok {
		productType = pt
	}

	switch property {
	case vangogh_integration.OwnedProperty:
		if owned {
			switch productType {
			case vangogh_integration.GameProductType:
				if productDownloadStatus == vangogh_integration.DownloadStatusUnknown &&
					productValidationStatus == vangogh_integration.ValidationStatusUnknown {
					fmtBadge.Icon = compton.CircleDashed
				} else if productDownloadStatus == vangogh_integration.DownloadStatusDownloaded ||
					productDownloadStatus == vangogh_integration.DownloadStatusValidated {
					fmtBadge.Icon = compton.CircleCompactDisk
				}
			default:
				fmtBadge.Icon = compton.CircleCompactDisk
			}
		}
	case vangogh_integration.DownloadQueuedProperty:
		if productDownloadStatus == vangogh_integration.DownloadStatusQueued {
			fmtBadge.Icon = compton.CircleClockArrows
		}
	case vangogh_integration.DownloadStartedProperty:
		if productDownloadStatus == vangogh_integration.DownloadStatusDownloading {
			fmtBadge.Icon = compton.CircleDownwardArrow
		}
	case vangogh_integration.ProductValidationResultProperty:
		if owned && productType == vangogh_integration.GameProductType {
			if vrSymbol, ok := compton_data.ValidationStatusSymbols[productValidationStatus]; ok {
				fmtBadge.Icon = vrSymbol
			}
		}
	case vangogh_integration.TopPercentProperty:
		if rdx.HasKey(vangogh_integration.TopPercentProperty, id) {
			fmtBadge.Icon = compton.Trophy
		}
	case vangogh_integration.UserWishlistProperty:
		if wish, ok := rdx.GetLastVal(vangogh_integration.UserWishlistProperty, id); ok && wish == vangogh_integration.TrueValue {
			fmtBadge.Icon = compton.Heart
		}
	case vangogh_integration.PreOrderProperty:
		if po, ok := rdx.GetLastVal(vangogh_integration.PreOrderProperty, id); ok && po == vangogh_integration.TrueValue {
			fmtBadge.Title = "PO"
		}
	case vangogh_integration.InDevelopmentProperty:
		if inDev, ok := rdx.GetLastVal(vangogh_integration.InDevelopmentProperty, id); ok && inDev == vangogh_integration.TrueValue {
			fmtBadge.Title = "IN DEV"
		}
	case vangogh_integration.IsDemoProperty:
		if demo, ok := rdx.GetLastVal(vangogh_integration.IsDemoProperty, id); ok && demo == vangogh_integration.TrueValue {
			fmtBadge.Title = "DEMO"
		}
	case vangogh_integration.IsModProperty:
		if mod, ok := rdx.GetLastVal(vangogh_integration.IsModProperty, id); ok && mod == vangogh_integration.TrueValue {
			fmtBadge.Icon = compton.PuzzlePiece
		}
	case vangogh_integration.IsFreeProperty:
		if demo, ok := rdx.GetLastVal(vangogh_integration.IsDemoProperty, id); ok && demo == vangogh_integration.TrueValue {
			fmtBadge.Title = ""
		} else if mod, ok := rdx.GetLastVal(vangogh_integration.IsModProperty, id); ok && mod == vangogh_integration.TrueValue {
			fmtBadge.Title = ""
		} else if free, ok := rdx.GetLastVal(vangogh_integration.IsFreeProperty, id); ok && free == vangogh_integration.TrueValue {
			fmtBadge.Title = "FREE"
		}
	case vangogh_integration.ComingSoonProperty:
		if owned {
			fmtBadge.Title = ""
		} else if soon, ok := rdx.GetLastVal(vangogh_integration.ComingSoonProperty, id); ok && soon == vangogh_integration.TrueValue {
			fmtBadge.Title = "SOON"
		}
	case vangogh_integration.ProductTypeProperty:
		if pt, ok := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); ok && pt != vangogh_integration.GameProductType {
			switch pt {
			case vangogh_integration.PackProductType:
				fmtBadge.Icon = compton.TwoStackedItems
			case vangogh_integration.DlcProductType:
				fmtBadge.Icon = compton.ItemPlus
			}
		}
	case vangogh_integration.DiscountPercentageProperty:
		if owned {
			fmtBadge.Title = ""
		} else if dp, ok := rdx.GetLastVal(vangogh_integration.DiscountPercentageProperty, id); ok && dp != "0" {
			fmtBadge.Title = "-" + dp + "%"
		} else {
			fmtBadge.Title = ""
		}
	case vangogh_integration.TagIdProperty:
		if tagId, ok := rdx.GetLastVal(vangogh_integration.TagIdProperty, id); ok {
			if tagName, sure := rdx.GetLastVal(vangogh_integration.TagNameProperty, tagId); sure {
				fmtBadge.Title = tagName
			}
		}
	case vangogh_integration.LocalTagsProperty:
		if localTags, ok := rdx.GetAllValues(vangogh_integration.LocalTagsProperty, id); ok {
			fmtBadge.Title = strings.Join(localTags, ", ")
		}
	case vangogh_integration.StoreTagsProperty:
		if rdx.HasValue(vangogh_integration.StoreTagsProperty, id, compton_data.GogPreservationProgramTag) {
			fmtBadge.Icon = compton.Gemstone
		} else {
			fmtBadge.Title = ""
		}
	}
	return fmtBadge
}
