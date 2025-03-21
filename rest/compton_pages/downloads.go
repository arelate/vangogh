package compton_pages

import (
	"fmt"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_data"
	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/font_weight"
	"github.com/boggydigital/compton/consts/size"
	"github.com/boggydigital/redux"
	"slices"
	"strconv"
	"strings"
)

type DownloadVariant struct {
	dlType   vangogh_integration.DownloadType
	version  string
	langCode string
}

var downloadTypesStrings = map[vangogh_integration.DownloadType]string{
	vangogh_integration.Installer: "Installer",
	vangogh_integration.DLC:       "DLC",
	vangogh_integration.Extra:     "Extra",
	vangogh_integration.Movie:     "Movie",
}

var downloadTypesColors = map[vangogh_integration.DownloadType]color.Color{
	vangogh_integration.Installer: color.Purple,
	vangogh_integration.DLC:       color.Indigo,
	vangogh_integration.Extra:     color.Orange,
	vangogh_integration.Movie:     color.Red,
}

var validationResultsFontWeights = map[vangogh_integration.ValidationResult]font_weight.Weight{
	vangogh_integration.ValidationResultUnknown:        font_weight.Normal,
	vangogh_integration.ValidatedSuccessfully:          font_weight.Bolder,
	vangogh_integration.ValidatedWithGeneratedChecksum: font_weight.Normal,
	vangogh_integration.ValidatedUnresolvedManualUrl:   font_weight.Normal,
	vangogh_integration.ValidatedMissingLocalFile:      font_weight.Normal,
	vangogh_integration.ValidatedMissingChecksum:       font_weight.Normal,
	vangogh_integration.ValidationError:                font_weight.Bolder,
	vangogh_integration.ValidatedChecksumMismatch:      font_weight.Bolder,
}

// Downloads will present available installers, DLCs in the following hierarchy:
// - Operating system heading - Installers and DLCs (separately)
// - title_values list of downloads by version
func Downloads(id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.InstallersSection)

	pageStack := compton.FlexItems(s, direction.Column)
	s.Append(pageStack)

	if owned, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && owned == vangogh_integration.FalseValue {
		ownershipRequiredNotice := compton.Fspan(s, "Downloads are available for owned products only").
			ForegroundColor(color.Gray)
		pageStack.Append(ownershipRequiredNotice)
		return s
	}

	if valRes := validationResults(s, id, dls, rdx); valRes != nil {
		pageStack.Append(valRes)
	}

	dlOs := downloadsOperatingSystems(dls)

	for _, os := range dlOs {

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
				if dlLinks := downloadLinks(s, os, productTitle, variant, dls, rdx); dlLinks != nil {
					pageStack.Append(dlLinks)

				}
			}

			if jj != len(productTitles)-1 {
				pageStack.Append(compton.Hr())
			}
		}
	}

	return s
}

func validationResults(r compton.Registrar, id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) compton.Element {

	hasInstallerDlcs := false
	for _, dl := range dls {
		if dl.Type != vangogh_integration.Extra {
			hasInstallerDlcs = true
			break
		}
	}

	if !hasInstallerDlcs {
		return nil
	}

	pvrc := color.Gray
	if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
		pvr := vangogh_integration.ParseValidationResult(pvrs)
		pvrc = compton_fragments.ValidationResultsColors[pvr]
	}

	valRes := compton.Frow(r).FontSize(size.Small).
		IconColor(compton.Circle, pvrc).
		Heading("Installers, DLC")
	results := make(map[vangogh_integration.ValidationResult]int)

	for _, dl := range dls {
		// only display installers, DLCs validation summary
		if dl.Type != vangogh_integration.Installer && dl.Type != vangogh_integration.DLC {
			continue
		}
		vr := vangogh_integration.ValidationResultUnknown
		if muss, ok := rdx.GetLastVal(vangogh_integration.ManualUrlStatusProperty, dl.ManualUrl); ok && vangogh_integration.ParseManualUrlStatus(muss) == vangogh_integration.ManualUrlValidated {
			if vrs, sure := rdx.GetLastVal(vangogh_integration.ManualUrlValidationResultProperty, dl.ManualUrl); sure {
				vr = vangogh_integration.ParseValidationResult(vrs)
			}
		}
		results[vr] = results[vr] + 1
	}

	for _, vr := range vangogh_integration.ValidationResultsOrder {
		if result, ok := results[vr]; ok && result > 0 {
			valRes.PropVal(vr.HumanReadableString(), strconv.Itoa(result))
		}
	}

	return compton.FICenter(r, valRes)
}

func operatingSystemHeading(r compton.Registrar, os vangogh_integration.OperatingSystem) compton.Element {
	osRow := compton.FlexItems(r, direction.Row).
		AlignItems(align.Center).
		JustifyContent(align.Center).
		ColumnGap(size.Small).
		BackgroundColor(color.Background)
	osRow.AddClass("operating-system-heading")
	osSymbol := compton.Sparkle
	if smb, ok := compton_data.OperatingSystemSymbols[os]; ok {
		osSymbol = smb
	}
	osIcon := compton.SvgUse(r, osSymbol)
	osIcon.AddClass("operating-system")
	osTitle := compton.H3()
	osString := ""
	switch os {
	case vangogh_integration.AnyOperatingSystem:
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

func downloadLinks(r compton.Registrar,
	os vangogh_integration.OperatingSystem,
	productTitle string,
	dv *DownloadVariant,
	dls vangogh_integration.DownloadsList,
	rdx redux.Readable) compton.Element {

	downloads := filterDownloads(os, dls, productTitle, dv)

	dsTitle := "Download link"
	if len(downloads) > 1 {
		dsTitle = fmt.Sprintf("%d download links", len(downloads))
	}

	//dsHeading := compton.Fspan(r, dsTitle).FontSize(size.Small)

	dsDownloadLinks := compton.DSSmall(r, dsTitle, false)

	downloadsColumn := compton.FlexItems(r, direction.Column).
		RowGap(size.Normal)
	dsDownloadLinks.Append(downloadsColumn)

	for ii, dl := range downloads {
		if link := downloadLink(r, productTitle, dl, rdx); link != nil {
			downloadsColumn.Append(link)
		}
		if ii != len(downloads)-1 {
			downloadLinksHr := compton.Hr()
			downloadLinksHr.AddClass("subtle")
			downloadsColumn.Append(downloadLinksHr)
		}
	}

	return dsDownloadLinks
}

func downloadLink(r compton.Registrar, productTitle string, dl vangogh_integration.Download, rdx redux.Readable) compton.Element {

	link := compton.A("/files?manual-url=" + dl.ManualUrl)
	link.AddClass("download", dl.Type.String())

	linkColumn := compton.FlexItems(r, direction.Column).
		RowGap(size.Small)

	name := dl.Name
	if dl.Type == vangogh_integration.DLC {
		name = dl.ProductTitle
	}

	namePrefix := ""
	if strings.Contains(name, productTitle) {
		namePrefix = productTitle
	}
	nameSuffix := strings.TrimPrefix(name, productTitle)

	linkTitle := compton.FlexItems(r, direction.Row).ColumnGap(size.XSmall).FontWeight(font_weight.Normal)

	if namePrefix != "" {
		linkPrefix := compton.Fspan(r, namePrefix).ForegroundColor(color.Gray)
		linkTitle.Append(linkPrefix)
	}
	if nameSuffix != "" {
		linkSuffix := compton.Fspan(r, nameSuffix).ForegroundColor(color.Foreground)
		linkTitle.Append(linkSuffix)
	}

	linkColumn.Append(linkTitle)

	vr := vangogh_integration.ValidationResultUnknown

	if muss, ok := rdx.GetLastVal(vangogh_integration.ManualUrlStatusProperty, dl.ManualUrl); ok && vangogh_integration.ParseManualUrlStatus(muss) == vangogh_integration.ManualUrlValidated {
		if vrs, sure := rdx.GetLastVal(vangogh_integration.ManualUrlValidationResultProperty, dl.ManualUrl); sure {
			vr = vangogh_integration.ParseValidationResult(vrs)
		}
	}

	validationResult := compton.Fspan(r, vr.HumanReadableString()).
		FontSize(size.Small).
		ForegroundColor(compton_fragments.ValidationResultsColors[vr]).
		FontWeight(validationResultsFontWeights[vr])
	linkColumn.Append(validationResult)

	sizeFr := compton.Frow(r).FontSize(size.Small).
		PropVal("Size", fmtBytes(dl.EstimatedBytes))
	linkColumn.Append(sizeFr)

	link.Append(linkColumn)

	return link
}

func downloadsOperatingSystems(dls vangogh_integration.DownloadsList) []vangogh_integration.OperatingSystem {
	dlOs := make(map[vangogh_integration.OperatingSystem]any)
	for _, dl := range dls {
		dlOs[dl.OS] = nil
	}

	oses := make([]vangogh_integration.OperatingSystem, 0, len(dlOs))
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

func getProductTitles(os vangogh_integration.OperatingSystem, dls vangogh_integration.DownloadsList) []string {
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

func getDownloadVariants(os vangogh_integration.OperatingSystem, title string, dls vangogh_integration.DownloadsList) []*DownloadVariant {

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

func filterDownloads(os vangogh_integration.OperatingSystem, dls vangogh_integration.DownloadsList, productTitle string, dv *DownloadVariant) []vangogh_integration.Download {
	downloads := make([]vangogh_integration.Download, 0)
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
