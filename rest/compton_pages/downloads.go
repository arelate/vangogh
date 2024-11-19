package compton_pages

import (
	"fmt"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/kevlar"
	"slices"
	"strconv"
	"strings"
)

type DownloadVariant struct {
	dlType   vangogh_local_data.DownloadType
	version  string
	langCode string
}

var downloadTypesStrings = map[vangogh_local_data.DownloadType]string{
	vangogh_local_data.Installer: "Installer",
	vangogh_local_data.DLC:       "DLC",
	vangogh_local_data.Extra:     "Extra",
	vangogh_local_data.Movie:     "Movie",
}

var downloadTypesColors = map[vangogh_local_data.DownloadType]color.Color{
	vangogh_local_data.Installer: color.Purple,
	vangogh_local_data.DLC:       color.Indigo,
	vangogh_local_data.Extra:     color.Orange,
	vangogh_local_data.Movie:     color.Red,
}

// Downloads will present available installers, DLCs in the following hierarchy:
// - Operating system heading - Installers and DLCs (separately)
// - title_values list of downloads by version
func Downloads(id string, dls vangogh_local_data.DownloadsList, rdx kevlar.ReadableRedux) compton.PageElement {

	//if err := json.NewEncoder(os.Stdout).Encode(dls); err != nil {
	//	panic(err)
	//}

	s := compton_fragments.ProductSection(compton_data.DownloadsSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if owned, ok := rdx.GetLastVal(vangogh_local_data.OwnedProperty, id); ok && owned == vangogh_local_data.FalseValue {
		ownershipRequiredNotice := compton.Fspan(s, "Downloads are available for owned products only").
			ForegroundColor(color.Gray)
		pageStack.Append(ownershipRequiredNotice)
		return s
	}

	if valRes := validationResults(s, id, rdx); valRes != nil {
		pageStack.Append(valRes)
	}

	dlOs := downloadsOperatingSystems(dls)

	for ii, os := range dlOs {

		if osHeading := operatingSystemHeading(s, os); osHeading != nil {
			pageStack.Append(osHeading)
		}

		productTitles := getProductTitles(os, dls)
		for jj, productTitle := range productTitles {

			titleHeadings := compton.H4Text(productTitle)
			pageStack.Append(titleHeadings)

			variants := getDownloadVariants(os, productTitle, dls)

			for _, variant := range variants {
				if dv := downloadVariant(s, variant); dv != nil {
					pageStack.Append(dv)
				}
				if dlLinks := downloadLinks(s, os, productTitle, variant, dls); dlLinks != nil {
					pageStack.Append(dlLinks)

				}
			}

			if jj != len(productTitles)-1 {
				pageStack.Append(compton.Hr())
			}
		}

		if ii != len(dlOs)-1 {
			thickHr := compton.Hr()
			thickHr.AddClass("thick")
			pageStack.Append(thickHr)
		}
	}

	return s
}

func validationResults(r compton.Registrar, id string, rdx kevlar.ReadableRedux) compton.Element {
	if valDate, ok := rdx.GetLastVal(vangogh_local_data.ValidationCompletedProperty, id); ok {
		if valRes, sure := rdx.GetAllValues(vangogh_local_data.ValidationResultProperty, id); sure && len(valRes) > 0 {

			lastResult := valRes[len(valRes)-1]
			valSect := compton.FlexItems(r, direction.Row).
				JustifyContent(align.Center).
				ColumnGap(size.Small).
				FontSize(size.Small)
			valSect.AddClass("validation-results", lastResult)

			var valDateElement compton.Element

			if vd, err := strconv.ParseInt(valDate, 10, 64); err == nil {
				valDateElement = compton.Fspan(r, compton_fragments.EpochDate(vd)).
					ForegroundColor(color.Gray)
			}

			valResTitle := ""
			valResColor := color.Gray
			switch lastResult {
			case "OK":
				valResTitle = "Validation successful"
				valResColor = color.Green
			case "missing-checksum":
				valResTitle = "Missing checksum"
				valResColor = color.Mint
			case "unresolved-manual-url":
				valResTitle = "Unresolved URL"
				valResColor = color.Teal
			case "missing-download":
				valResTitle = "Missing download"
				valResColor = color.Yellow
			case "failed-validation":
				valResTitle = "Failed validation"
				valResColor = color.Red
			case "":
				valResTitle = "Not validated yet"
			default:
				valResTitle = "Unknown result"
				valResColor = color.Gray
			}

			valResElement := compton.Fspan(r, valResTitle).
				FontWeight(font_weight.Bolder).
				ForegroundColor(valResColor)

			if valDateElement != nil {
				valSect.Append(valDateElement)
			}
			valSect.Append(valResElement)

			return valSect

		}
	}

	return nil
}

func operatingSystemHeading(r compton.Registrar, os vangogh_local_data.OperatingSystem) compton.Element {
	osRow := compton.FlexItems(r, direction.Row).
		AlignItems(align.Center).
		ColumnGap(size.Small)
	osSymbol := compton.Sparkle
	if smb, ok := compton_data.OperatingSystemSymbols[os]; ok {
		osSymbol = smb
	}
	osIcon := compton.SvgUse(r, osSymbol)
	osIcon.AddClass("operating-system")
	osTitle := compton.H3()
	osString := ""
	switch os {
	case vangogh_local_data.AnyOperatingSystem:
		osString = "Goodies"
	default:
		osString = os.String()
	}
	osTitle.Append(compton.Fspan(r, osString))
	osRow.Append(osIcon, osTitle)
	return osRow
}

func downloadVariant(r compton.Registrar, dv *DownloadVariant) compton.Element {

	fr := compton.Frow(r).
		FontSize(size.Small).
		IconColor(compton.Circle, downloadTypesColors[dv.dlType]).
		Heading(downloadTypesStrings[dv.dlType])

	if dv.langCode != "" {
		fr.PropVal("Lang", compton_data.LanguageFlags[dv.langCode])
	}
	if dv.version != "" {
		fr.PropVal("Version", dv.version)
	}

	return fr
}

func downloadLinks(r compton.Registrar, os vangogh_local_data.OperatingSystem, productTitle string, dv *DownloadVariant, dls vangogh_local_data.DownloadsList) compton.Element {

	downloads := filterDownloads(os, dls, productTitle, dv)

	dsTitle := "Download link"
	if len(downloads) > 1 {
		dsTitle = fmt.Sprintf("%d download links", len(downloads))
	}

	dsHeading := compton.Fspan(r, dsTitle).FontSize(size.Small)

	dsDownloadLinks := compton.DSSmall(r, dsHeading, false)

	downloadsColumn := compton.FlexItems(r, direction.Column).
		RowGap(size.Normal)
	dsDownloadLinks.Append(downloadsColumn)

	for ii, dl := range downloads {
		if link := downloadLink(r, productTitle, dl); link != nil {
			downloadsColumn.Append(link)
		}
		if ii != len(downloads)-1 {
			downloadsColumn.Append(compton.Hr())
		}
	}

	return dsDownloadLinks
}

func downloadLink(r compton.Registrar, productTitle string, dl vangogh_local_data.Download) compton.Element {

	link := compton.A("/files?manual-url=" + dl.ManualUrl)
	link.AddClass("download", dl.Type.String())

	linkColumn := compton.FlexItems(r, direction.Column).
		RowGap(size.Small)

	name := dl.Name
	if dl.Type == vangogh_local_data.DLC {
		name = dl.ProductTitle
	}

	namePrefix := ""
	if strings.Contains(name, productTitle) {
		namePrefix = productTitle
	}
	nameSuffix := strings.TrimPrefix(name, productTitle)

	linkPrefix := compton.Fspan(r, namePrefix).ForegroundColor(color.Gray)
	linkSuffix := compton.Fspan(r, nameSuffix).ForegroundColor(color.Foreground)

	linkTitle := compton.FlexItems(r, direction.Row).ColumnGap(size.XSmall).FontWeight(font_weight.Normal)

	linkTitle.Append(linkPrefix, linkSuffix)
	linkColumn.Append(linkTitle)

	sizeFr := compton.Frow(r).
		FontSize(size.Small).
		PropVal("Size", fmtBytes(dl.EstimatedBytes))
	linkColumn.Append(sizeFr)

	link.Append(linkColumn)

	return link
}

func downloadsOperatingSystems(dls vangogh_local_data.DownloadsList) []vangogh_local_data.OperatingSystem {
	dlOs := make(map[vangogh_local_data.OperatingSystem]any)
	for _, dl := range dls {
		dlOs[dl.OS] = nil
	}

	oses := make([]vangogh_local_data.OperatingSystem, 0, len(dlOs))
	for _, os := range compton_data.OSOrder {
		if _, ok := dlOs[os]; ok {
			oses = append(oses, os)
		}
	}
	return oses
}

func (dv *DownloadVariant) Equals(other *DownloadVariant) bool {
	return dv.dlType == other.dlType &&
		dv.version == other.version &&
		dv.langCode == other.langCode
}

func hasDownloadVariant(dvs []*DownloadVariant, other *DownloadVariant) bool {
	for _, dv := range dvs {
		if dv.Equals(other) {
			return true
		}
	}
	return false
}

func getProductTitles(os vangogh_local_data.OperatingSystem, dls vangogh_local_data.DownloadsList) []string {
	titles := make([]string, 0)
	for _, dl := range dls {
		if dl.OS != os {
			continue
		}

		if !slices.Contains(titles, dl.ProductTitle) {
			titles = append(titles, dl.ProductTitle)
		}
	}
	return titles
}

func getDownloadVariants(os vangogh_local_data.OperatingSystem, title string, dls vangogh_local_data.DownloadsList) []*DownloadVariant {

	variants := make([]*DownloadVariant, 0)
	for _, dl := range dls {
		if dl.OS != os {
			continue
		}
		if dl.ProductTitle != title {
			continue
		}

		dv := &DownloadVariant{
			dlType:   dl.Type,
			version:  dl.Version,
			langCode: dl.LanguageCode,
		}

		if !hasDownloadVariant(variants, dv) {
			variants = append(variants, dv)
		}

	}
	return variants
}

func filterDownloads(os vangogh_local_data.OperatingSystem, dls vangogh_local_data.DownloadsList, productTitle string, dv *DownloadVariant) []vangogh_local_data.Download {
	downloads := make([]vangogh_local_data.Download, 0)
	for _, dl := range dls {
		if dl.OS != os ||
			dl.Type != dv.dlType ||
			dv.version != dl.Version ||
			dv.langCode != dl.LanguageCode ||
			productTitle != dl.ProductTitle {
			continue
		}
		downloads = append(downloads, dl)
	}
	return downloads
}

func fmtBytes(b int) string {
	const unit = 1000
	if b < unit {
		return fmt.Sprintf("%d B", b)
	}
	div, exp := int64(unit), 0
	for n := b / unit; n >= unit; n /= unit {
		div *= unit
		exp++
	}
	return fmt.Sprintf("%.1f %cB",
		float64(b)/float64(div), "kMGTPE"[exp])
}
