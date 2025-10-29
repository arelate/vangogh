package compton_pages

import (
	"fmt"
	"net/url"
	"slices"
	"strings"
	"time"

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
)

type DownloadVariant struct {
	downloadType     vangogh_integration.DownloadType
	version          string
	langCode         string
	estimatedBytes   int64
	manualUrlStatus  vangogh_integration.ManualUrlStatus
	validationResult vangogh_integration.ValidationResult
}

// Installers will present available installers, DLCs in the following hierarchy:
// - Operating system heading - Installers and DLCs (separately)
// - title_values list of downloads by version
func Installers(id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.InstallersSection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)
	s.Append(pageStack)

	if downloadCompleted, ok, err := rdx.ParseLastValTime(vangogh_integration.DownloadCompletedProperty, id); ok && err == nil {
		dcFrow := compton.Frow(s).FontSize(size.XSmall)
		dcFrow.PropVal("Downloaded", downloadCompleted.Local().Format(time.RFC1123))
		pageStack.Append(compton.FICenter(s, dcFrow))
	}

	if owned, ok := rdx.GetLastVal(vangogh_integration.OwnedProperty, id); ok && owned == vangogh_integration.FalseValue {
		ownershipRequiredNotice := compton.Fspan(s, "Installers are available for owned products only").
			ForegroundColor(color.RepGray)
		pageStack.Append(ownershipRequiredNotice)
		return s
	}

	dlOs := downloadsOperatingSystems(dls)

	for _, os := range dlOs {

		if osHeading := operatingSystemHeading(s, os); osHeading != nil {
			pageStack.Append(osHeading)
		}

		productTitles := getProductTitles(os, dls)
		for jj, productTitle := range productTitles {

			productStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)
			pageStack.Append(productStack)

			titleHeadings := compton.H3Text(productTitle)
			productStack.Append(titleHeadings)

			variants := getDownloadVariants(os, productTitle, dls, rdx)

			for _, variant := range variants {
				if dv := downloadVariant(s, variant); dv != nil {
					productStack.Append(dv)
				}
				if dlLinks := downloadLinks(s, id, os, productTitle, variant, dls, rdx); dlLinks != nil {
					productStack.Append(dlLinks)
				}
			}

			if jj != len(productTitles)-1 {
				pageStack.Append(compton.Hr())
			}
		}
	}

	return s
}

func operatingSystemHeading(r compton.Registrar, os vangogh_integration.OperatingSystem) compton.Element {
	osRow := compton.FlexItems(r, direction.Row).
		AlignItems(align.Center).
		JustifyContent(align.Center).
		ColumnGap(size.Small)

	osSymbol := compton.Sparkle
	if smb, ok := compton_data.OperatingSystemSymbols[os]; ok {
		osSymbol = smb
	}
	osIcon := compton.SvgUse(r, osSymbol)

	osString := ""
	switch os {
	case vangogh_integration.AnyOperatingSystem:
		osString = "Goodies"
	default:
		osString = os.String()
	}

	osRow.Append(osIcon, compton.Text(osString))

	return compton.SectionDivider(r, osRow)
}

func downloadVariant(r compton.Registrar, dv *DownloadVariant) compton.Element {

	fr := compton.Frow(r).FontSize(size.XSmall)

	fr.IconColor(compton_data.DownloadTypesSymbols[dv.downloadType], color.RepGray)
	fr.Heading(dv.downloadType.HumanReadableString())

	if dv.langCode != "" {
		fr.PropVal("Lang", compton_data.LanguageFlags[dv.langCode])
	}
	if dv.version != "" {
		fr.PropVal("Version", dv.version)
	}
	if dv.estimatedBytes > 0 {
		fr.PropVal("Size", vangogh_integration.FormatBytes(dv.estimatedBytes))
	}

	var downloadValidationStatus string
	var downloadValidationSymbol compton.Symbol
	downloadValidationColor := color.RepGray

	if dv.downloadType == vangogh_integration.Installer || dv.downloadType == vangogh_integration.DLC {

		switch dv.validationResult {
		case vangogh_integration.ValidationResultUnknown:
			downloadValidationStatus = dv.manualUrlStatus.HumanReadableString()
			downloadValidationSymbol = compton_data.ManualUrlStatusSymbols[dv.manualUrlStatus]
		default:
			downloadValidationStatus = dv.validationResult.HumanReadableString()
			downloadValidationColor = compton_data.ValidationResultsColors[dv.validationResult]
			downloadValidationSymbol = compton_data.ValidationResultsSymbols[dv.validationResult]
		}

	} else {
		downloadValidationStatus = dv.manualUrlStatus.HumanReadableString()
	}

	if downloadValidationSymbol != compton.NoSymbol {
		fr.IconColor(downloadValidationSymbol, downloadValidationColor)
	}

	fr.Elements(compton.Fspan(r, downloadValidationStatus).
		FontSize(size.XSmall).
		ForegroundColor(downloadValidationColor))

	return fr
}

func downloadLinks(r compton.Registrar,
	id string,
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

	dsDownloadLinks := compton.DSSmall(r, dsTitle, false)

	downloadsColumn := compton.FlexItems(r, direction.Column).RowGap(size.Normal)
	dsDownloadLinks.Append(downloadsColumn)

	for ii, dl := range downloads {
		if link := downloadLink(r, id, productTitle, dl, rdx); link != nil {
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

func downloadLink(r compton.Registrar,
	id string,
	productTitle string,
	dl vangogh_integration.Download,
	rdx redux.Readable) compton.Element {

	q := url.Values{}
	q.Set("id", id)
	q.Set("download-type", dl.Type.String())
	q.Set("manual-url", dl.ManualUrl)

	linkContainer := compton.FlexItems(r, direction.Column).RowGap(size.Small)

	link := compton.A("/files?" + q.Encode())
	link.AddClass("download", dl.Type.String())

	linkContainer.Append(link)

	linkColumn := compton.FlexItems(r, direction.Column).RowGap(size.Small)

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
		linkPrefix := compton.Fspan(r, namePrefix).ForegroundColor(color.RepGray)
		linkTitle.Append(linkPrefix)
	}
	if nameSuffix != "" {
		linkSuffix := compton.Fspan(r, nameSuffix).ForegroundColor(color.RepForeground)
		linkTitle.Append(linkSuffix)
	}

	linkColumn.Append(linkTitle)

	dvs := vangogh_integration.NewDvs(dl.ManualUrl, rdx)

	manualUrlStatusValidationResultRow := compton.FlexItems(r, direction.Row).ColumnGap(size.Small)

	var manualUrlStatusValidationResult string
	manualUrlStatusValidationResultSymbol := compton.NoSymbol
	manualUrlStatusValidationResultColor := color.RepGray

	if dl.Type == vangogh_integration.Installer || dl.Type == vangogh_integration.DLC {

		manualUrlStatusValidationResult = dvs.HumanReadableString()

		switch dvs.ValidationResult() {
		case vangogh_integration.ValidationResultUnknown:
			manualUrlStatusValidationResultSymbol = compton_data.ManualUrlStatusSymbols[dvs.ManualUrlStatus()]
		default:
			manualUrlStatusValidationResultColor = compton_data.ValidationResultsColors[dvs.ValidationResult()]
			manualUrlStatusValidationResultSymbol = compton_data.ValidationResultsSymbols[dvs.ValidationResult()]
		}
	} else {
		manualUrlStatusValidationResult = dvs.ManualUrlStatus().HumanReadableString()
	}

	if manualUrlStatusValidationResultSymbol != compton.NoSymbol {
		manualUrlStatusValidationResultRow.Append(
			compton.SvgUse(r, manualUrlStatusValidationResultSymbol).ForegroundColor(manualUrlStatusValidationResultColor))
	}

	manualUrlStatusValidationResultRow.Append(compton.Fspan(r, manualUrlStatusValidationResult).
		FontSize(size.XSmall).
		ForegroundColor(manualUrlStatusValidationResultColor))

	linkColumn.Append(manualUrlStatusValidationResultRow)

	link.Append(linkColumn)

	bottomRow := compton.FlexItems(r, direction.Row).ColumnGap(size.Small).AlignItems(align.Center)

	var sizeFr *compton.FrowElement

	if dl.EstimatedBytes > 0 {
		sizeFr = compton.Frow(r).FontSize(size.XSmall)
		sizeFr.PropVal("Size", vangogh_integration.FormatBytes(dl.EstimatedBytes))
		bottomRow.Append(sizeFr)
	}

	copyManualUrlToClipboard := compton.CopyToClipboard(r,
		compton.Fspan(r, "Copy Manual URL").FontSize(size.XSmall).ForegroundColor(color.Blue).FontWeight(font_weight.Bolder),
		compton.Fspan(r, "Copied!").FontSize(size.XSmall).ForegroundColor(color.Green),
		compton.Fspan(r, "Error").FontSize(size.XSmall).ForegroundColor(color.Red),
		dl.ManualUrl)
	bottomRow.Append(copyManualUrlToClipboard)

	linkContainer.Append(bottomRow)

	return linkContainer
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
	return dv.downloadType == other.downloadType &&
		dv.version == other.version &&
		dv.langCode == other.langCode
}

func getDownloadVariant(dvs []*DownloadVariant, other *DownloadVariant) *DownloadVariant {
	for _, dv := range dvs {
		if dv.Equals(other) {
			return dv
		}
	}
	return nil
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

func getDownloadVariants(os vangogh_integration.OperatingSystem, title string, dls vangogh_integration.DownloadsList, rdx redux.Readable) []*DownloadVariant {

	variants := make([]*DownloadVariant, 0)
	for _, dl := range dls {
		if dl.OS != os {
			continue
		}
		if dl.ProductTitle != title {
			continue
		}

		dvs := vangogh_integration.NewDvs(dl.ManualUrl, rdx)

		dv := &DownloadVariant{
			downloadType:     dl.Type,
			version:          dl.Version,
			langCode:         dl.LanguageCode,
			estimatedBytes:   dl.EstimatedBytes,
			validationResult: dvs.ValidationResult(),
			manualUrlStatus:  dvs.ManualUrlStatus(),
		}

		if edv := getDownloadVariant(variants, dv); edv == nil {
			variants = append(variants, dv)
		} else {
			edv.estimatedBytes += dl.EstimatedBytes
			// use the "worst" validation result, worse = larger value
			if edv.validationResult < dvs.ValidationResult() {
				edv.validationResult = dvs.ValidationResult()
			}
			// use the "worst" manual url status, worse = larger value
			if edv.manualUrlStatus < dvs.ManualUrlStatus() {
				edv.manualUrlStatus = dvs.ManualUrlStatus()
			}
		}

	}
	return variants
}

func filterDownloads(os vangogh_integration.OperatingSystem, dls vangogh_integration.DownloadsList, productTitle string, dv *DownloadVariant) []vangogh_integration.Download {
	downloads := make([]vangogh_integration.Download, 0)
	for _, dl := range dls {
		if dl.OS != os ||
			dl.Type != dv.downloadType ||
			dv.version != dl.Version ||
			dv.langCode != dl.LanguageCode ||
			productTitle != dl.ProductTitle {
			continue
		}
		downloads = append(downloads, dl)
	}
	return downloads
}
