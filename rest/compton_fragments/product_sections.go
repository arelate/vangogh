package compton_fragments

import (
	"slices"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/perm"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/author"
	"github.com/boggydigital/redux"
)

func ProductSections(id string, rdx redux.Readable, permissions ...author.Permission) []string {

	hasSections := make([]string, 0)

	hasSections = append(hasSections, compton_data.InfoSection)

	if sr, ok := rdx.GetLastVal(vangogh_integration.SummaryRatingProperty, id); ok && sr != "" {
		hasSections = append(hasSections, compton_data.ReceptionSection)
	}

	offeringsCount := 0
	for _, rpp := range compton_data.OfferingsProperties {
		if rps, ok := rdx.GetAllValues(rpp, id); ok {
			offeringsCount += len(rps)
		}
	}

	if offeringsCount > 0 {
		hasSections = append(hasSections, compton_data.OfferingsSection)
	}

	if rdx.HasKey(vangogh_integration.ScreenshotsProperty, id) ||
		rdx.HasKey(vangogh_integration.VideoIdProperty, id) {
		hasSections = append(hasSections, compton_data.MediaSection)
	}

	if rdx.HasValue(vangogh_integration.TypesProperty, id, vangogh_integration.SteamAppNews.String()) ||
		rdx.HasKey(vangogh_integration.ChangelogProperty, id) {
		hasSections = append(hasSections, compton_data.NewsSection)
	}

	if sdc, ok := rdx.GetLastVal(vangogh_integration.SteamDeckAppCompatibilityCategoryProperty, id); ok && sdc != "Unknown" {
		hasSections = append(hasSections, compton_data.CompatibilitySection)
	} else if pt, sure := rdx.GetLastVal(vangogh_integration.ProtonDBTierProperty, id); sure && pt != "" {
		hasSections = append(hasSections, compton_data.CompatibilitySection)
	}

	if slices.Contains(permissions, perm.ReadFiles) {
		if val, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && val == vangogh_integration.TrueValue {
			if productType, _ := rdx.GetLastVal(vangogh_integration.ProductTypeProperty, id); productType != vangogh_integration.DlcProductType &&
				productType != vangogh_integration.PackProductType {
				if preorder, yeah := rdx.GetLastVal(vangogh_integration.PreOrderProperty, id); yeah && preorder == vangogh_integration.TrueValue {
					// do nothing
				} else {
					hasSections = append(hasSections, compton_data.InstallersSection)
				}
			}
		}
	}

	return hasSections
}
