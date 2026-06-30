package compton_fragments

import (
	"fmt"
	"net/url"
	"slices"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
)

const propertyValuesLimit = 2

type formattedProperty struct {
	values  map[string]string
	class   string
	actions map[string]string
}

func ProductProperties(r compton.Registrar, id string, rdx redux.Readable, properties []string, permissions ...author.Permission) []compton.Element {

	productProperties := make([]compton.Element, 0)

	ppo := compton_data.PermittedProperties(properties, permissions...)

	for property := range ppo {

		fmtProperty := formatProperty(id, property, rdx)
		if tv := propertyTitleValues(r, property, fmtProperty, permissions...); tv != nil {
			productProperties = append(productProperties, tv)
		}
	}

	return productProperties
}

func hrefSearch(property, value string) string {

	q := make(url.Values)
	q.Set(property, value)

	u := new(url.URL{
		Path: "/search",
	})

	u.RawQuery = q.Encode()

	return u.String()
}

func hrefSearchCredits(value string) string {

	q := make(url.Values)
	q.Set(vangogh_integration.CreditsProperty, value)

	u := new(url.URL{
		Path: "/search",
	})

	u.RawQuery = q.Encode()

	return u.String()
}

func HrefSearchDescSortGogGlobalReleaseDate(property, value string) string {

	q := make(url.Values)
	q.Set(vangogh_integration.SortProperty, vangogh_integration.GogGlobalReleaseDateProperty)
	q.Set(vangogh_integration.DescendingProperty, vangogh_integration.TrueValue)
	q.Set(property, value)

	u := new(url.URL{
		Path: "/search",
	})

	u.RawQuery = q.Encode()

	return u.String()
}

func hrefEmpty() string {
	return ""
}

func formatProperty(id, property string, rdx redux.Readable) formattedProperty {

	fmtProperty := formattedProperty{
		actions: make(map[string]string),
		values:  make(map[string]string),
	}

	owned := false
	if lp, ok := rdx.GetLastVal(vangogh_integration.GogOwnedProperty, id); ok {
		owned = lp == vangogh_integration.TrueValue
	}
	isFree := false
	if ifp, ok := rdx.GetLastVal(vangogh_integration.GogIsFreeProperty, id); ok {
		isFree = ifp == vangogh_integration.TrueValue
	}
	isDiscounted := false
	if idp, ok := rdx.GetLastVal(vangogh_integration.GogIsDiscountedProperty, id); ok {
		isDiscounted = idp == vangogh_integration.TrueValue
	}

	var hltbId string
	if hid, ok := rdx.GetLastVal(vangogh_integration.GogHltbIdProperty, id); ok {
		hltbId = hid
	}
	var opencriticId string
	if ocid, ok := rdx.GetLastVal(vangogh_integration.GogOpenCriticIdProperty, id); ok {
		opencriticId = ocid
	}
	var metacriticId string
	if mcid, ok := rdx.GetLastVal(vangogh_integration.GogMetacriticIdProperty, id); ok {
		metacriticId = mcid
	}
	var steamAppId string
	if said, ok := rdx.GetLastVal(vangogh_integration.GogSteamAppIdProperty, id); ok {
		steamAppId = said
	}

	var values []string
	if pvs, ok := rdx.GetAllValues(property, id); ok {
		values = pvs
	}

	firstValue := ""
	if len(values) > 0 {
		firstValue = values[0]
	}

	switch property {
	case vangogh_integration.GogUserWishlistProperty:
		if owned && firstValue != vangogh_integration.TrueValue {
			break
		}
		title := "No"
		if firstValue == vangogh_integration.TrueValue {
			title = "Yes"
		}
		fmtProperty.values[title] = hrefSearch(property, firstValue)
	case vangogh_integration.GogOrderDateProperty:
		for _, value := range values {
			jtd := formatDate(value)
			if d, _, ok := strings.Cut(value, "T"); ok {
				fmtProperty.values[jtd] = hrefSearch(property, d)
			} else {
				fmtProperty.values[jtd] = hrefSearch(property, value)
			}
		}
	case vangogh_integration.LanguageCodeProperty:
		for _, value := range values {
			fmtProperty.values[compton_data.FormatLanguage(value)] = hrefSearch(property, value)
		}
	case vangogh_integration.GogRatingProperty:
		for _, value := range values {
			fmtProperty.values[fmtGOGRating(value)] = hrefEmpty()
		}
	case vangogh_integration.GogTagIdProperty:
		for _, value := range values {
			tagName := value
			if tnp, ok := rdx.GetLastVal(vangogh_integration.GogTagNameProperty, value); ok {
				tagName = tnp
			}
			fmtProperty.values[tagName] = hrefSearch(property, value)
		}
	case vangogh_integration.GogPriceProperty:
		for _, value := range values {
			if !isFree {
				if isDiscounted && !owned {
					if bpp, ok := rdx.GetLastVal(vangogh_integration.GogBasePriceProperty, id); ok {
						fmtProperty.values["Base: "+bpp] = hrefEmpty()
					}
					fmtProperty.values["Sale: "+value] = hrefEmpty()
				} else {
					fmtProperty.values[value] = hrefEmpty()
				}
			}
		}
	case vangogh_integration.HltbHoursToCompleteMainProperty:
		fallthrough
	case vangogh_integration.HltbHoursToCompletePlusProperty:
		fallthrough
	case vangogh_integration.HltbHoursToComplete100Property:
		if hltbValues, sure := rdx.GetAllValues(property, hltbId); sure {
			for _, value := range hltbValues {
				ct := strings.TrimLeft(value, "0") + " hrs"
				fmtProperty.values[ct] = hrefEmpty()
			}
		}
	case vangogh_integration.HltbPlatformsProperty:
		if hltbPlatforms, sure := rdx.GetAllValues(property, hltbId); sure {
			for _, platform := range hltbPlatforms {
				fmtProperty.values[platform] = hrefEmpty()
			}
		}
	case vangogh_integration.HltbReviewScoreProperty:
		if hltbRating, sure := rdx.GetLastVal(property, hltbId); sure {
			if !isNotPositiveRating(hltbRating) {
				fmtProperty.values[fmtHltbRating(hltbRating)] = hrefEmpty()
			}
		}
	case vangogh_integration.GogDiscountPercentageProperty:
		for _, value := range values {
			fmtProperty.values[value] = hrefEmpty()
		}
	case vangogh_integration.GogPublishersProperty:
		fallthrough
	case vangogh_integration.GogDevelopersProperty:
		for _, value := range values {
			fmtProperty.values[value] = HrefSearchDescSortGogGlobalReleaseDate(property, value)
		}
	case vangogh_integration.PcgwEnginesProperty:
		if pcgwPageId, ok := rdx.GetLastVal(vangogh_integration.GogPcgwPageIdProperty, id); ok && pcgwPageId != "" {
			if pcgwEngines, sure := rdx.GetAllValues(vangogh_integration.PcgwEnginesProperty, pcgwPageId); sure {
				for _, value := range pcgwEngines {
					fmtProperty.values[value] = hrefEmpty()
				}
			}
		}
	case vangogh_integration.SteamReviewScoreProperty:
		if srs, ok := rdx.GetLastVal(property, steamAppId); ok && srs != "" {
			if !isNotPositiveRating(srs) {
				fmtProperty.values[fmtSteamRating(srs)] = hrefEmpty()
			}
		}
	case vangogh_integration.OpenCriticMedianScoreProperty:
		if ocms, ok := rdx.GetLastVal(property, opencriticId); ok && ocms != "" {
			if !isNotPositiveRating(ocms) {
				fmtProperty.values[FmtRating(ocms)] = hrefEmpty()
			}
		}
	case vangogh_integration.MetacriticScoreProperty:
		if mcs, ok := rdx.GetLastVal(property, metacriticId); ok && mcs != "" {
			if !isNotPositiveRating(mcs) {
				fmtProperty.values[FmtRating(mcs)] = hrefEmpty()
			}
		}
	case vangogh_integration.OpenCriticPercentileProperty:
		if ocp, ok := rdx.GetLastVal(property, opencriticId); ok && ocp != "" {
			if !isNotPositiveRating(ocp) {
				fmtProperty.values[ocp] = hrefSearch(property, url.QueryEscape(ocp))
			}
		}
	case vangogh_integration.CreatorsProperty:
		fallthrough
	case vangogh_integration.DirectorsProperty:
		fallthrough
	case vangogh_integration.ProducersProperty:
		fallthrough
	case vangogh_integration.DesignersProperty:
		fallthrough
	case vangogh_integration.ProgrammersProperty:
		fallthrough
	case vangogh_integration.ArtistsProperty:
		fallthrough
	case vangogh_integration.WritersProperty:
		fallthrough
	case vangogh_integration.ComposersProperty:
		for _, value := range values {
			fmtProperty.values[value] = hrefSearchCredits(value)
		}
	case vangogh_integration.SteamCategoriesProperty:
		if scs, ok := rdx.GetAllValues(vangogh_integration.SteamCategoriesProperty, steamAppId); ok {
			for _, value := range scs {
				fmtProperty.values[value] = hrefEmpty()
			}
		}
	case vangogh_integration.SteamSteamOsAppCompatibilityCategoryProperty:
		fallthrough
	case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
		if scc, ok := rdx.GetLastVal(property, steamAppId); ok && scc != "" {
			fmtProperty.values[scc] = hrefEmpty()
		}
	case vangogh_integration.ProtonDbTierProperty:
		if pt, ok := rdx.GetLastVal(property, steamAppId); ok && pt != "" {
			fmtProperty.values[pt] = hrefEmpty()
		}
	case vangogh_integration.ProtonDbConfidenceProperty:
		if pc, ok := rdx.GetLastVal(property, steamAppId); ok && pc != "" {
			fmtProperty.values[pc] = hrefEmpty()
		}
	default:
		for _, value := range values {
			if value != "" {
				fmtProperty.values[value] = hrefSearch(property, value)
			}
		}
	}

	// format actions, class
	switch property {
	case vangogh_integration.GogOwnedProperty:
		if res, ok := rdx.GetLastVal(vangogh_integration.GogProductValidationResultProperty, id); ok {
			fmtProperty.class = res
		}
	case vangogh_integration.GogUserWishlistProperty:
		if !owned || firstValue == vangogh_integration.TrueValue {
			switch firstValue {
			case vangogh_integration.TrueValue:
				fmtProperty.actions["Remove"] = "/wishlist/remove?id=" + id
			case vangogh_integration.FalseValue:
				fallthrough
			default:
				fmtProperty.actions["Add"] = "/wishlist/add?id=" + id
			}
		}
	case vangogh_integration.GogTagIdProperty:
		if owned {
			fmtProperty.actions["Edit"] = "/tags/edit?id=" + id
		}
	case vangogh_integration.VangoghLocalTagsProperty:
		fmtProperty.actions["Edit"] = "/local-tags/edit?id=" + id
	case vangogh_integration.SteamReviewScoreDescProperty:
		if srsd, ok := rdx.GetLastVal(property, steamAppId); ok && srsd != "" {
			fmtProperty.class = ReviewClass(srsd)
		}
	case vangogh_integration.GogRatingProperty:
		fmtProperty.class = ReviewClass(fmtGOGRating(firstValue))
	case vangogh_integration.HltbReviewScoreProperty:
		if hrs, ok := rdx.GetLastVal(property, hltbId); ok && hrs != "" {
			fmtProperty.class = ReviewClass(fmtHltbRating(hrs))
		}
	case vangogh_integration.SteamReviewScoreProperty:
		if srs, ok := rdx.GetLastVal(property, steamAppId); ok && srs != "" {
			fmtProperty.class = ReviewClass(fmtSteamRating(srs))
		}
	case vangogh_integration.OpenCriticTierProperty:
		if oct, ok := rdx.GetLastVal(property, opencriticId); ok {
			fmtProperty.class = oct
		}
	case vangogh_integration.OpenCriticPercentileProperty:
		if _, ok := rdx.GetLastVal(property, opencriticId); ok {
			fmtProperty.class = vangogh_integration.RatingPositive
		}
	case vangogh_integration.OpenCriticMedianScoreProperty:
		if ocms, ok := rdx.GetLastVal(property, opencriticId); ok {
			fmtProperty.class = ReviewClass(FmtRating(ocms))
		}
	case vangogh_integration.MetacriticScoreProperty:
		if ms, ok := rdx.GetLastVal(property, metacriticId); ok {
			fmtProperty.class = ReviewClass(FmtRating(ms))
		}
	case vangogh_integration.SteamSteamOsAppCompatibilityCategoryProperty:
		fallthrough
	case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
		if scc, ok := rdx.GetLastVal(property, steamAppId); ok && scc != "" {
			fmtProperty.class = scc
		}
	case vangogh_integration.ProtonDbTierProperty:
		if pt, ok := rdx.GetLastVal(property, steamAppId); ok && pt != "" {
			fmtProperty.class = pt
		}
	case vangogh_integration.ProtonDbConfidenceProperty:
		if pc, ok := rdx.GetLastVal(property, steamAppId); ok && pc != "" {
			fmtProperty.class = pc
		}
	}

	return fmtProperty
}

func propertyTitleValues(r compton.Registrar, property string, fmtProperty formattedProperty, permissions ...author.Permission) *compton.TitleValuesElement {

	if len(fmtProperty.values) == 0 && len(fmtProperty.actions) == 0 {
		return nil
	}

	tv := compton.TitleValues(r, compton_data.PropertyTitles[property]).
		SetLinksTarget(compton.LinkTargetTop).
		ForegroundColor(color.Inherit).
		TitleForegroundColor(color.Gray).
		RowGap(size.XSmall).
		Width(size.XXXLarge)

	if len(fmtProperty.values) > 0 {
		tv.AppendLinkValues(propertyValuesLimit, fmtProperty.values)

		if fmtProperty.class != "" {
			tv.AddClass(fmtProperty.class)
		}
	}

	if prm, ok := compton_data.PropertyActionPermissions[property]; ok && !slices.Contains(permissions, prm) {
		return tv
	}

	if len(fmtProperty.actions) > 0 {
		for ac, acHref := range fmtProperty.actions {
			actionLink := compton.A(acHref)
			actionLink.SetAttribute("target", "_top")
			actionLink.Append(compton.Fspan(r, ac).ForegroundColor(color.Foreground).Width(size.Initial))
			tv.Append(actionLink)
		}
	}

	return tv
}

func ReviewClass(sr string) string {
	if strings.Contains(sr, vangogh_integration.RatingPositive) {
		return vangogh_integration.RatingPositive
	} else if strings.Contains(sr, vangogh_integration.RatingNegative) {
		return vangogh_integration.RatingNegative
	} else if strings.Contains(sr, vangogh_integration.RatingMixed) {
		return vangogh_integration.RatingMixed
	} else {
		return vangogh_integration.RatingUnknown
	}
}

func fmtGOGRating(rs string) string {
	rd := ""
	if ri, err := strconv.ParseInt(rs, 10, 32); err == nil {
		rd = vangogh_integration.RatingDesc(ri * 2)
		if ri > 0 {
			rd += fmt.Sprintf(" %.0f%%", float32(ri*2))
		}
	}
	return rd
}

func fmtHltbRating(rs string) string {
	rd := ""
	if ri, err := strconv.ParseInt(rs, 10, 32); err == nil {
		rd = vangogh_integration.RatingDesc(ri)
		if ri > 0 {
			rd += fmt.Sprintf(" %d%%", ri)
		}
	}
	return rd
}

func fmtSteamRating(rs string) string {
	rd := ""
	if ri, err := strconv.ParseInt(rs, 10, 32); err == nil {
		rd = vangogh_integration.RatingDesc(ri * 10)
		if ri > 0 {
			rd += fmt.Sprintf(" %d%%", ri*10)
		}
	}
	return rd

}

func FmtRatingValue(rs string) string {
	var rv string
	if rf, err := strconv.ParseFloat(rs, 64); err == nil {
		ri := int64(rf)
		if ri > 0 {
			rv = fmt.Sprintf(" %d%%", ri)
		}
	}
	return rv
}

func FmtRating(rs string) string {
	rd := ""
	if rf, err := strconv.ParseFloat(rs, 64); err == nil {
		ri := int64(rf)
		rd = vangogh_integration.RatingDesc(ri)
		if ri > 0 {
			rd += fmt.Sprintf(" %d%%", ri)
		}
	}
	return rd
}

func formatDate(s string) string {
	if t, err := time.Parse(time.RFC3339, s); err == nil {
		return t.Format("2006.01.02")
	}
	return s
}

func isNotPositiveRating(v string) bool {
	return v == "0" || v == "-1" || v == ""
}
