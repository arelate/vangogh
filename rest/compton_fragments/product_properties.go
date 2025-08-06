package compton_fragments

import (
	"fmt"
	"net/url"
	"strconv"
	"strings"
	"time"

	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
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

func ProductProperties(r compton.Registrar, id string, rdx redux.Readable, properties ...string) []compton.Element {

	productProperties := make([]compton.Element, 0)

	for _, property := range properties {

		fmtProperty := formatProperty(id, property, rdx)
		if tv := propertyTitleValues(r, property, fmtProperty); tv != nil {
			productProperties = append(productProperties, tv)
		}
	}

	return productProperties
}

func searchHref(property, value string) string {
	return fmt.Sprintf("/search?%s=%s", property, value)
}

func searchCreditsHref(value string) string {
	return fmt.Sprintf("/search?credits=%s", value)
}

func grdSortedSearchHref(property, value string) string {
	return fmt.Sprintf("/search?%s=%s&sort=global-release-date&desc=true", property, value)
}

func noHref() string {
	return ""
}

func formatProperty(id, property string, rdx redux.Readable) formattedProperty {

	fmtProperty := formattedProperty{
		actions: make(map[string]string),
		values:  make(map[string]string),
	}

	owned := false
	if lp, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok {
		owned = lp == vangogh_integration.TrueValue
	}
	isFree := false
	if ifp, ok := rdx.GetLastVal(vangogh_integration.IsFreeProperty, id); ok {
		isFree = ifp == vangogh_integration.TrueValue
	}
	isDiscounted := false
	if idp, ok := rdx.GetLastVal(vangogh_integration.IsDiscountedProperty, id); ok {
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
	case vangogh_integration.UserWishlistProperty:
		if owned && firstValue != vangogh_integration.TrueValue {
			break
		}
		title := "No"
		if firstValue == vangogh_integration.TrueValue {
			title = "Yes"
		}
		fmtProperty.values[title] = searchHref(property, firstValue)
	case vangogh_integration.GOGOrderDateProperty:
		for _, value := range values {
			jtd := formatDate(value)
			if d, _, ok := strings.Cut(value, "T"); ok {
				fmtProperty.values[jtd] = searchHref(property, d)
			} else {
				fmtProperty.values[jtd] = searchHref(property, value)
			}
		}
	case vangogh_integration.LanguageCodeProperty:
		for _, value := range values {
			fmtProperty.values[compton_data.FormatLanguage(value)] = searchHref(property, value)
		}
	case vangogh_integration.RatingProperty:
		for _, value := range values {
			fmtProperty.values[fmtGOGRating(value)] = noHref()
		}
	case vangogh_integration.TagIdProperty:
		for _, value := range values {
			tagName := value
			if tnp, ok := rdx.GetLastVal(vangogh_integration.TagNameProperty, value); ok {
				tagName = tnp
			}
			fmtProperty.values[tagName] = searchHref(property, value)
		}
	case vangogh_integration.PriceProperty:
		for _, value := range values {
			if !isFree {
				if isDiscounted && !owned {
					if bpp, ok := rdx.GetLastVal(vangogh_integration.BasePriceProperty, id); ok {
						fmtProperty.values["Base: "+bpp] = noHref()
					}
					fmtProperty.values["Sale: "+value] = noHref()
				} else {
					fmtProperty.values[value] = noHref()
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
			fmtProperty.values[ct] = noHref()
		}
	case vangogh_integration.HltbReviewScoreProperty:
		if firstValue != "0" {
			fmtProperty.values[fmtHltbRating(firstValue)] = noHref()
		}
	case vangogh_integration.DiscountPercentageProperty:
		for _, value := range values {
			fmtProperty.values[value] = noHref()
		}
	case vangogh_integration.PublishersProperty:
		fallthrough
	case vangogh_integration.DevelopersProperty:
		for _, value := range values {
			fmtProperty.values[value] = grdSortedSearchHref(property, value)
		}
	case vangogh_integration.EnginesBuildsProperty:
		for _, value := range values {
			fmtProperty.values[value] = noHref()
		}
	case vangogh_integration.SteamReviewScoreProperty:
		if firstValue != "" {
			fmtProperty.values[fmtSteamRating(firstValue)] = noHref()
		}
	case vangogh_integration.OpenCriticMedianScoreProperty:
		fallthrough
	case vangogh_integration.MetacriticScoreProperty:
		fallthrough
	case vangogh_integration.TopPercentProperty:
		if firstValue != "" {
			fmtProperty.values[firstValue] = searchHref(property, url.QueryEscape(firstValue))
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
			if has, ok := rdx.GetLastVal(vangogh_integration.HasMultipleCreditsProperty, value); ok && has == vangogh_integration.TrueValue {
				fmtProperty.values[value] = searchCreditsHref(value)
			} else {
				fmtProperty.values[value] = noHref()
			}
		}
	default:
		for _, value := range values {
			if value != "" {
				fmtProperty.values[value] = searchHref(property, value)
			}
		}
	}

	// format actions, class
	switch property {
	case vangogh_integration.OwnedProperty:
		if res, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
			fmtProperty.class = res
		}
	case vangogh_integration.UserWishlistProperty:
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
	case vangogh_integration.TagIdProperty:
		if owned {
			fmtProperty.actions["Edit"] = "/tags/edit?id=" + id
		}
	case vangogh_integration.LocalTagsProperty:
		fmtProperty.actions["Edit"] = "/local-tags/edit?id=" + id
	case vangogh_integration.SteamReviewScoreDescProperty:
		fmtProperty.class = reviewClass(firstValue)
	case vangogh_integration.RatingProperty:
		fmtProperty.class = reviewClass(fmtGOGRating(firstValue))
	case vangogh_integration.HltbReviewScoreProperty:
		fmtProperty.class = reviewClass(fmtHltbRating(firstValue))
	case vangogh_integration.SteamReviewScoreProperty:
		fmtProperty.class = reviewClass(fmtSteamRating(firstValue))
	case vangogh_integration.OpenCriticTierProperty:
		fmtProperty.class = firstValue
	case vangogh_integration.TopPercentProperty:
		fmtProperty.class = vangogh_integration.RatingPositive
	case vangogh_integration.OpenCriticMedianScoreProperty:
		fallthrough
	case vangogh_integration.MetacriticScoreProperty:
		fallthrough
	case vangogh_integration.SteamOsAppCompatibilityCategoryProperty:
		fallthrough
	case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
		fmtProperty.class = firstValue
	}

	return fmtProperty
}

func propertyTitleValues(r compton.Registrar, property string, fmtProperty formattedProperty) *compton.TitleValuesElement {

	if len(fmtProperty.values) == 0 && len(fmtProperty.actions) == 0 {
		return nil
	}

	tv := compton.TitleValues(r, compton_data.PropertyTitles[property]).
		SetLinksTarget(compton.LinkTargetTop).
		ForegroundColor(color.Inherit).
		TitleForegroundColor(color.RepGray).
		RowGap(size.XSmall)

	if len(fmtProperty.values) > 0 {
		tv.AppendLinkValues(propertyValuesLimit, fmtProperty.values)

		if fmtProperty.class != "" {
			tv.AddClass(fmtProperty.class)
		}
	}

	if len(fmtProperty.actions) > 0 {
		for ac, acHref := range fmtProperty.actions {
			actionLink := compton.A(acHref)
			actionLink.SetAttribute("target", "_top")
			actionLink.Append(compton.Fspan(r, ac).ForegroundColor(color.Blue))
			tv.Append(actionLink)
		}
	}

	return tv
}

func reviewClass(sr string) string {
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

func FmtAggregatedRating(rs string) string {
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
