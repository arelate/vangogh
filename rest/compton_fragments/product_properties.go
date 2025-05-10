package compton_fragments

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"maps"
	"net/url"
	"slices"
	"strconv"
	"strings"
	"time"
)

type formattedProperty struct {
	values  map[string]string
	class   string
	actions map[string]string
}

func ProductProperties(r compton.Registrar, id string, rdx redux.Readable, properties ...string) []compton.Element {

	productProperties := make([]compton.Element, 0)

	for _, property := range properties {

		if property == vangogh_integration.OperatingSystemsProperty {
			if tv := operatingSystemsTitleValues(r, id, rdx); tv != nil {
				productProperties = append(productProperties, tv)
				continue
			}
		}

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

	values, _ := rdx.GetAllValues(property, id)
	firstValue := ""
	if len(values) > 0 {
		firstValue = values[0]
	}

	for _, value := range values {
		switch property {
		case vangogh_integration.UserWishlistProperty:
			if owned {
				break
			}
			title := "No"
			if value == vangogh_integration.TrueValue {
				title = "Yes"
			}
			fmtProperty.values[title] = searchHref(property, value)
		case vangogh_integration.GOGOrderDateProperty:
			jtd := formatDate(value)
			if d, _, ok := strings.Cut(value, "T"); ok {
				fmtProperty.values[jtd] = searchHref(property, d)
			} else {
				fmtProperty.values[jtd] = searchHref(property, value)
			}
		case vangogh_integration.LanguageCodeProperty:
			fmtProperty.values[compton_data.FormatLanguage(value)] = searchHref(property, value)
		case vangogh_integration.RatingProperty:
			fmtProperty.values[fmtGOGRating(value)] = noHref()
		case vangogh_integration.TagIdProperty:
			tagName := value
			if tnp, ok := rdx.GetLastVal(vangogh_integration.TagNameProperty, value); ok {
				tagName = tnp
			}
			fmtProperty.values[tagName] = searchHref(property, value)
		case vangogh_integration.PriceProperty:
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
		case vangogh_integration.HltbHoursToCompleteMainProperty:
			fallthrough
		case vangogh_integration.HltbHoursToCompletePlusProperty:
			fallthrough
		case vangogh_integration.HltbHoursToComplete100Property:
			ct := strings.TrimLeft(value, "0") + " hrs"
			fmtProperty.values[ct] = noHref()
		case vangogh_integration.HltbReviewScoreProperty:
			if value != "0" {
				fmtProperty.values[fmtHltbRating(value)] = noHref()
			}
		case vangogh_integration.DiscountPercentageProperty:
			fmtProperty.values[value] = noHref()
		case vangogh_integration.PublishersProperty:
			fallthrough
		case vangogh_integration.DevelopersProperty:
			fmtProperty.values[value] = grdSortedSearchHref(property, value)
		case vangogh_integration.EnginesBuildsProperty:
			fmtProperty.values[value] = noHref()
		case vangogh_integration.SteamReviewScoreProperty:
			if value != "" {
				fmtProperty.values[fmtSteamRating(value)] = noHref()
			}
		case vangogh_integration.OpenCriticMedianScoreProperty:
			fallthrough
		case vangogh_integration.MetacriticScoreProperty:
			fallthrough
		case vangogh_integration.AggregatedRatingProperty:
			if value != "" {
				fmtProperty.values[FmtAggregatedRating(value)] = noHref()
			}
		case vangogh_integration.TopPercentProperty:
			fmtProperty.values[value] = searchHref(property, url.QueryEscape(value))
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
			if has, ok := rdx.GetLastVal(vangogh_integration.HasMultipleCreditsProperty, value); ok && has == vangogh_integration.TrueValue {
				fmtProperty.values[value] = searchCreditsHref(value)
			} else {
				fmtProperty.values[value] = noHref()
			}
		default:
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
		if !owned {
			switch firstValue {
			case vangogh_integration.TrueValue:
				fmtProperty.actions["Remove"] = "/wishlist/remove?id=" + id
			case vangogh_integration.FalseValue:
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
	case vangogh_integration.AggregatedRatingProperty:
		fmtProperty.class = reviewClass(FmtAggregatedRating(firstValue))
	case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
		fmtProperty.class = firstValue
	}

	return fmtProperty
}

func operatingSystemsTitleValues(r compton.Registrar, id string, rdx redux.Readable) compton.Element {
	property := vangogh_integration.OperatingSystemsProperty
	propertyTitle := compton_data.PropertyTitles[property]
	tv := compton.TitleValues(r, propertyTitle).RowGap(size.XSmall).TitleForegroundColor(color.Gray)
	row := compton.FlexItems(r, direction.Row).JustifyContent(align.Start)
	tv.Append(row)
	if values, ok := rdx.GetAllValues(property, id); ok {
		oses := vangogh_integration.ParseManyOperatingSystems(values)
		for _, os := range []vangogh_integration.OperatingSystem{
			vangogh_integration.Windows, vangogh_integration.MacOS, vangogh_integration.Linux} {
			if !slices.Contains(oses, os) {
				continue
			}
			osLink := compton.A(searchHref(property, os.String()))
			osLink.SetAttribute("target", "_top")
			osLink.Append(compton.SvgUse(r, compton_data.OperatingSystemSymbols[os]))
			row.Append(osLink)
		}
	}
	return tv
}

func propertyTitleValues(r compton.Registrar, property string, fmtProperty formattedProperty) *compton.TitleValuesElement {

	if len(fmtProperty.values) == 0 && len(fmtProperty.actions) == 0 {
		return nil
	}

	tv := compton.TitleValues(r, compton_data.PropertyTitles[property]).
		SetLinksTarget(compton.LinkTargetTop).
		ForegroundColor(color.Foreground).
		TitleForegroundColor(color.Gray).
		RowGap(size.XSmall)

	if len(fmtProperty.values) > 0 {

		if len(fmtProperty.values) < 4 {
			tv.AppendLinkValues(fmtProperty.values)
		} else {
			summaryTitle := fmt.Sprintf("%d values", len(fmtProperty.values))
			//summaryElement := compton.Fspan(r, summaryTitle).
			//	ForegroundColor(color.Foreground)
			ds := compton.DSSmall(r, summaryTitle, false).
				SummaryMarginBlockEnd(size.Normal).
				DetailsMarginBlockEnd(size.Small)
			row := compton.FlexItems(r, direction.Row).
				JustifyContent(align.Start)
			sortedKeys := slices.Sorted(maps.Keys(fmtProperty.values))
			for _, link := range sortedKeys {
				href := fmtProperty.values[link]
				anchor := compton.AText(link, href)
				anchor.SetAttribute("target", "_top")
				row.Append(anchor)
			}
			ds.Append(row)
			tv.AppendValues(ds)
		}

		if fmtProperty.class != "" {
			tv.AddClass(fmtProperty.class)
		}
	}

	if len(fmtProperty.actions) > 0 {
		for ac, acHref := range fmtProperty.actions {
			actionLink := compton.A(acHref)
			actionLink.SetAttribute("target", "_top")
			actionLink.Append(compton.Fspan(r, ac).ForegroundColor(color.Blue))
			tv.AppendValues(actionLink)
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
