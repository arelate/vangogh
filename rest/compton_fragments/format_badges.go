package compton_fragments

import (
	"strconv"
	"strings"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
)

func FormatBadges(id string, rdx redux.Readable, badgeProperties []string, permissions ...author.Permission) []*compton.FormattedBadge {

	fmtBadges := make([]*compton.FormattedBadge, 0, len(badgeProperties))

	ppo := compton_data.PermittedProperties(badgeProperties, permissions...)

	for p := range ppo {
		if fmtBadge := formatBadge(id, p, rdx); fmtBadge.Title != "" || fmtBadge.Icon != compton.NoSymbol {
			fmtBadges = append(fmtBadges, fmtBadge)
		}
	}

	return fmtBadges
}

func formatBadge(id, property string, rdx redux.Readable) *compton.FormattedBadge {

	fmtBadge := &compton.FormattedBadge{
		Color: color.Gray,
	}

	productDvs := vangogh_integration.NewProductDvs(id, rdx)
	productDownloadStatus := productDvs.DownloadStatus()
	productValidationStatus := productDvs.ValidationStatus()

	owned := false
	if lp, ok := rdx.GetLastVal(vangogh_integration.GogOwnedProperty, id); ok {
		owned = lp == vangogh_integration.TrueValue
	}

	var productType string
	if pt, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, id); ok {
		productType = pt
	}

	switch property {
	case vangogh_integration.GogOwnedProperty:
		if owned {
			switch productType {
			case gog_integration.ProductTypeGame:
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
	case vangogh_integration.VangoghDownloadQueuedProperty:
		if productDownloadStatus == vangogh_integration.DownloadStatusQueued {
			fmtBadge.Icon = compton.CircleClockArrows
		}
	case vangogh_integration.VangoghDownloadStartedProperty:
		if productDownloadStatus == vangogh_integration.DownloadStatusDownloading {
			fmtBadge.Icon = compton.CircleDownwardArrow
		}
	case vangogh_integration.GogProductValidationResultProperty:
		if owned && productType != gog_integration.ProductTypeDlc && productType != gog_integration.ProductTypePack {
			if vrSymbol, ok := compton_data.ValidationStatusSymbols[productValidationStatus]; ok {
				fmtBadge.Icon = vrSymbol

				if pgc, sure := rdx.GetLastVal(vangogh_integration.GogProductGeneratedChecksumProperty, id); sure && pgc == vangogh_integration.TrueValue {
					if fmtBadge.Icon == compton.HexagonPacked {
						fmtBadge.Icon = compton.HexagonNegativeDiagonalLine
					}
				}
			}
		}
	case vangogh_integration.OpenCriticPercentileProperty:
		if ocps, ok := rdx.GetLastVal(vangogh_integration.OpenCriticPercentileProperty, id); ok && ocps != "" {
			if ocpi, err := strconv.ParseInt(ocps, 10, 64); err == nil && ocpi >= 80 {
				fmtBadge.Icon = compton.Trophy
			}
		}
	case vangogh_integration.GogUserWishlistProperty:
		if wish, ok := rdx.GetLastVal(vangogh_integration.GogUserWishlistProperty, id); ok && wish == vangogh_integration.TrueValue {
			fmtBadge.Icon = compton.Heart
		}
	case vangogh_integration.GogPreOrderProperty:
		if po, ok := rdx.GetLastVal(vangogh_integration.GogPreOrderProperty, id); ok && po == vangogh_integration.TrueValue {
			fmtBadge.Title = "PO"
		}
	case vangogh_integration.GogInDevelopmentProperty:
		if inDev, ok := rdx.GetLastVal(vangogh_integration.GogInDevelopmentProperty, id); ok && inDev == vangogh_integration.TrueValue {
			fmtBadge.Title = "IN DEV"
		}
	case vangogh_integration.GogIsDemoProperty:
		if demo, ok := rdx.GetLastVal(vangogh_integration.GogIsDemoProperty, id); ok && demo == vangogh_integration.TrueValue {
			fmtBadge.Title = "DEMO"
		}
	case vangogh_integration.GogIsModProperty:
		if mod, ok := rdx.GetLastVal(vangogh_integration.GogIsModProperty, id); ok && mod == vangogh_integration.TrueValue {
			fmtBadge.Icon = compton.PuzzlePiece
		}
	case vangogh_integration.GogIsFreeProperty:
		if demo, ok := rdx.GetLastVal(vangogh_integration.GogIsDemoProperty, id); ok && demo == vangogh_integration.TrueValue {
			fmtBadge.Title = ""
		} else if mod, ok := rdx.GetLastVal(vangogh_integration.GogIsModProperty, id); ok && mod == vangogh_integration.TrueValue {
			fmtBadge.Title = ""
		} else if free, ok := rdx.GetLastVal(vangogh_integration.GogIsFreeProperty, id); ok && free == vangogh_integration.TrueValue {
			fmtBadge.Title = "FREE"
		}
	case vangogh_integration.GogComingSoonProperty:
		if owned {
			fmtBadge.Title = ""
		} else if soon, ok := rdx.GetLastVal(vangogh_integration.GogComingSoonProperty, id); ok && soon == vangogh_integration.TrueValue {
			fmtBadge.Title = "SOON"
		}
	case vangogh_integration.GogProductTypeProperty:
		if pt, ok := rdx.GetLastVal(vangogh_integration.GogProductTypeProperty, id); ok && pt != gog_integration.ProductTypeGame {
			switch pt {
			case gog_integration.ProductTypePack:
				fmtBadge.Icon = compton.ItemsPack
			case gog_integration.ProductTypeDlc:
				fmtBadge.Icon = compton.ItemPlus
			}
		}
	case vangogh_integration.GogDiscountPercentageProperty:
		if owned {
			fmtBadge.Title = ""
		} else if dp, ok := rdx.GetLastVal(vangogh_integration.GogDiscountPercentageProperty, id); ok && dp != "0" {
			fmtBadge.Title = "-" + dp + "%"
		} else {
			fmtBadge.Title = ""
		}
	case vangogh_integration.GogTagIdProperty:
		if tagId, ok := rdx.GetLastVal(vangogh_integration.GogTagIdProperty, id); ok {
			if tagName, sure := rdx.GetLastVal(vangogh_integration.GogTagNameProperty, tagId); sure {
				fmtBadge.Title = tagName
			}
		}
	case vangogh_integration.VangoghLocalTagsProperty:
		if localTags, ok := rdx.GetAllValues(vangogh_integration.VangoghLocalTagsProperty, id); ok {
			fmtBadge.Title = strings.Join(localTags, ", ")
		}
	case vangogh_integration.GogStoreTagsProperty:
		if rdx.HasValue(vangogh_integration.GogStoreTagsProperty, id, compton_data.GogPreservationProgramTag) {
			fmtBadge.Icon = compton.Gemstone
		} else {
			fmtBadge.Title = ""
		}
	}
	return fmtBadge
}
