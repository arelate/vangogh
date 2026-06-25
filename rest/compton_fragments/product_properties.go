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
		for _, value := range values {
			ct := strings.TrimLeft(value, "0") + " hrs"
			fmtProperty.values[ct] = hrefEmpty()
		}
	case vangogh_integration.HltbReviewScoreProperty:
		if !isNotPositiveRating(firstValue) {
			fmtProperty.values[fmtHltbRating(firstValue)] = hrefEmpty()
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
	case vangogh_integration.EnginesBuildsProperty:
		for _, value := range values {
			fmtProperty.values[value] = hrefEmpty()
		}
	case vangogh_integration.SteamReviewScoreProperty:
		if !isNotPositiveRating(firstValue) {
			fmtProperty.values[fmtSteamRating(firstValue)] = hrefEmpty()
		}
	case vangogh_integration.OpenCriticMedianScoreProperty:
		if !isNotPositiveRating(firstValue) {
			fmtProperty.values[FmtRating(firstValue)] = hrefEmpty()
		}
	case vangogh_integration.MetacriticScoreProperty:
		if !isNotPositiveRating(firstValue) {
			fmtProperty.values[FmtRating(firstValue)] = hrefEmpty()
		}
	case vangogh_integration.TopPercentProperty:
		if !isNotPositiveRating(firstValue) {
			fmtProperty.values[firstValue] = hrefSearch(property, url.QueryEscape(firstValue))
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
		fmtProperty.class = ReviewClass(firstValue)
	case vangogh_integration.GogRatingProperty:
		fmtProperty.class = ReviewClass(fmtGOGRating(firstValue))
	case vangogh_integration.HltbReviewScoreProperty:
		fmtProperty.class = ReviewClass(fmtHltbRating(firstValue))
	case vangogh_integration.SteamReviewScoreProperty:
		fmtProperty.class = ReviewClass(fmtSteamRating(firstValue))
	case vangogh_integration.OpenCriticTierProperty:
		fmtProperty.class = firstValue
	case vangogh_integration.TopPercentProperty:
		fmtProperty.class = vangogh_integration.RatingPositive
	case vangogh_integration.OpenCriticMedianScoreProperty:
		fallthrough
	case vangogh_integration.MetacriticScoreProperty:
		fmtProperty.class = ReviewClass(FmtRating(firstValue))
	case vangogh_integration.SteamSteamOsAppCompatibilityCategoryProperty:
		fallthrough
	case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
		fmtProperty.class = firstValue
	case vangogh_integration.ProtonDbTierProperty:
		fmtProperty.class = firstValue
	case vangogh_integration.ProtonDbConfidenceProperty:
		fmtProperty.class = firstValue
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
