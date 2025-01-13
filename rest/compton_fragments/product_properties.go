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
	"github.com/boggydigital/kevlar"
	"golang.org/x/exp/maps"
	"slices"
	"strconv"
	"strings"
)

type formattedProperty struct {
	values  map[string]string
	class   string
	actions map[string]string
}

func ProductProperties(r compton.Registrar, id string, rdx kevlar.ReadableRedux) compton.Element {
	grid := compton.GridItems(r).JustifyContent(align.Center)

	for _, property := range compton_data.ProductProperties {

		if property == vangogh_integration.OperatingSystemsProperty {
			if tv := operatingSystemsTitleValues(r, id, rdx); tv != nil {
				grid.Append(tv)
				continue
			}
		}

		fmtProperty := formatProperty(id, property, rdx)
		if tv := propertyTitleValues(r, property, fmtProperty); tv != nil {
			grid.Append(tv)
		}
	}

	return grid
}

func searchHref(property, value string) string {
	return fmt.Sprintf("/search?%s=%s", property, value)
}

func grdSortedSearchHref(property, value string) string {
	return fmt.Sprintf("/search?%s=%s&sort=global-release-date&desc=true", property, value)
}

func noHref() string {
	return ""
}

func formatProperty(id, property string, rdx kevlar.ReadableRedux) formattedProperty {

	fmtProperty := formattedProperty{
		actions: make(map[string]string),
		values:  make(map[string]string),
	}

	owned := false
	if op, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok {
		owned = op == vangogh_integration.TrueValue
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
		case vangogh_integration.WishlistedProperty:
			if owned {
				break
			}
			title := "No"
			if value == vangogh_integration.TrueValue {
				title = "Yes"
			}
			fmtProperty.values[title] = searchHref(property, value)
		case vangogh_integration.IncludesGamesProperty:
			fallthrough
		case vangogh_integration.IsIncludedByGamesProperty:
			fallthrough
		case vangogh_integration.RequiresGamesProperty:
			fallthrough
		case vangogh_integration.IsRequiredByGamesProperty:
			refTitle := value
			if rtp, ok := rdx.GetLastVal(vangogh_integration.TitleProperty, value); ok {
				refTitle = rtp
			}
			fmtProperty.values[refTitle] = "/product?id=" + value
		case vangogh_integration.GOGOrderDateProperty:
			jtd := justTheDate(value)
			fmtProperty.values[jtd] = searchHref(property, jtd)
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
		case vangogh_integration.HLTBHoursToCompleteMainProperty:
			fallthrough
		case vangogh_integration.HLTBHoursToCompletePlusProperty:
			fallthrough
		case vangogh_integration.HLTBHoursToComplete100Property:
			ct := strings.TrimLeft(value, "0") + " hrs"
			fmtProperty.values[ct] = noHref()
		case vangogh_integration.HLTBReviewScoreProperty:
			if value != "0" {
				fmtProperty.values[fmtHLTBRating(value)] = noHref()
			}
		case vangogh_integration.DiscountPercentageProperty:
			fmtProperty.values[value] = noHref()
		case vangogh_integration.PublishersProperty:
			fallthrough
		case vangogh_integration.DevelopersProperty:
			fmtProperty.values[value] = grdSortedSearchHref(property, value)
		case vangogh_integration.EnginesBuildsProperty:
			fmtProperty.values[value] = noHref()
		case vangogh_integration.AggregatedRatingProperty:
			if value != "" {
				fmtProperty.values[fmtAggregatedRating(value)] = noHref()
			}
		default:
			fmtProperty.values[value] = searchHref(property, value)
		}
	}

	// format actions, class
	switch property {
	case vangogh_integration.OwnedProperty:
		//if res, ok := rdx.GetLastVal(vangogh_integration.ValidationResultProperty, id); ok {
		//	fmtProperty.class = res
		//}
	case vangogh_integration.WishlistedProperty:
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
	case vangogh_integration.HLTBReviewScoreProperty:
		fmtProperty.class = reviewClass(fmtHLTBRating(firstValue))
	case vangogh_integration.AggregatedRatingProperty:
		fmtProperty.class = reviewClass(fmtAggregatedRating(firstValue))
	case vangogh_integration.SteamDeckAppCompatibilityCategoryProperty:
		fmtProperty.class = firstValue
		if firstValue != "" {
			fmtProperty.actions["&darr;"] = "#Steam Deck"
		}
	}

	return fmtProperty
}

func operatingSystemsTitleValues(r compton.Registrar, id string, rdx kevlar.ReadableRedux) compton.Element {
	property := vangogh_integration.OperatingSystemsProperty
	propertyTitle := compton_data.PropertyTitles[property]
	tv := compton.TitleValues(r, propertyTitle)
	row := compton.FlexItems(r, direction.Row).JustifyContent(align.Start)
	tv.Append(row)
	if values, ok := rdx.GetAllValues(property, id); ok {
		for _, os := range vangogh_integration.ParseManyOperatingSystems(values) {
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
		ForegroundColor(color.Gray).
		TitleForegroundColor(color.Foreground)

	if len(fmtProperty.values) > 0 {

		if len(fmtProperty.values) < 4 {
			tv.AppendLinkValues(fmtProperty.values)
		} else {
			summaryTitle := fmt.Sprintf("%d values", len(fmtProperty.values))
			summaryElement := compton.Fspan(r, summaryTitle).
				ForegroundColor(color.Foreground)
			ds := compton.DSSmall(r, summaryElement, false).
				SummaryMarginBlockEnd(size.Normal).
				DetailsMarginBlockEnd(size.Small)
			row := compton.FlexItems(r, direction.Row).
				JustifyContent(align.Start)
			keys := maps.Keys(fmtProperty.values)
			slices.Sort(keys)
			for _, link := range keys {
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
	if strings.Contains(sr, "Positive") {
		return "positive"
	} else if strings.Contains(sr, "Negative") {
		return "negative"
	} else {
		return "neutral"
	}
}

func fmtGOGRating(rs string) string {
	rd := ""
	if ri, err := strconv.ParseInt(rs, 10, 32); err == nil {
		rd = ratingDesc(ri * 2)
		if ri > 0 {
			rd += fmt.Sprintf(" (%.1f)", float32(ri)/10.0)
		}
	}
	return rd
}

func fmtHLTBRating(rs string) string {
	rd := ""
	if ri, err := strconv.ParseInt(rs, 10, 32); err == nil {
		rd = ratingDesc(ri)
		if ri > 0 {
			rd += fmt.Sprintf(" (%d)", ri)
		}
	}
	return rd
}

func fmtAggregatedRating(rs string) string {
	rd := ""
	if rf, err := strconv.ParseFloat(rs, 64); err == nil {
		ri := int64(rf)
		rd = ratingDesc(ri)
		if ri > 0 {
			rd += fmt.Sprintf(" (%d)", ri)
		}
	}
	return rd
}

func ratingDesc(ri int64) string {
	rd := "Not Rated"
	if ri >= 95 {
		rd = "Overwhelming Positive"
	} else if ri >= 85 {
		rd = "Very Positive"
	} else if ri >= 80 {
		rd = "Positive"
	} else if ri >= 70 {
		rd = "Mostly Positive"
	} else if ri >= 40 {
		rd = "Mixed"
	} else if ri >= 20 {
		rd = "Mostly Negative"
	} else if ri > 0 {
		rd = "Negative"
	}
	return rd
}

func justTheDate(s string) string {
	return strings.Split(s, " ")[0]
}
