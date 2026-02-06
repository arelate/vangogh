package compton_pages

import (
	"fmt"
	"net/url"
	"slices"
	"strconv"
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
	"github.com/boggydigital/nod"
	"github.com/boggydigital/redux"
)

type DownloadVariant struct {
	downloadType             vangogh_integration.DownloadType
	version                  string
	langCode                 string
	downloadedEstimatedBytes int64
	validatedEstimatedBytes  int64
	totalEstimatedBytes      int64
	downloadStatus           vangogh_integration.DownloadStatus
	generatedChecksum        bool
	validationStatus         vangogh_integration.ValidationStatus
}

// Installers will present available installers, DLCs in the following hierarchy:
// - Operating system heading - Installers and DLCs (separately)
// - title_values list of downloads by version
func Installers(id string, dls vangogh_integration.DownloadsList, rdx redux.Readable) compton.PageElement {

	s := compton_fragments.ProductSection(compton_data.InstallersSection, id, rdx)

	pageStack := compton.FlexItems(s, direction.Column).RowGap(size.Normal)
	s.Append(pageStack)

	var dvRow *compton.FrowElement

	if downloadCompleted, ok, err := rdx.ParseLastValTime(vangogh_integration.DownloadCompletedProperty, id); ok && err == nil {
		dvRow = compton.Frow(s).FontSize(size.XSmall)
		dvRow.PropVal("Downloaded", downloadCompleted.Local().Format(time.DateTime))
	} else if !ok {
		var downloadStarted time.Time
		if downloadStarted, ok, err = rdx.ParseLastValTime(vangogh_integration.DownloadStartedProperty, id); ok && err == nil {
			dvRow = compton.Frow(s).FontSize(size.XSmall)
			dvRow.PropVal("Download Started", downloadStarted.Local().Format(time.DateTime))
		} else if !ok {
			var downloadQueued time.Time
			if downloadQueued, ok, err = rdx.ParseLastValTime(vangogh_integration.DownloadQueuedProperty, id); ok && err == nil {
				dvRow = compton.Frow(s).FontSize(size.XSmall)
				dvRow.PropVal("Download Queued", downloadQueued.Local().Format(time.DateTime))
			}
		}
	}

	if validationDate, ok := rdx.GetLastVal(vangogh_integration.ProductValidationDateProperty, id); ok && validationDate != "" {
		if vdt, err := time.Parse(nod.TimeFormat, validationDate); err == nil {
			if dvRow == nil {
				dvRow = compton.Frow(s).FontSize(size.XSmall)
			}
			dvRow.PropVal("Validated", vdt.Local().Format(time.DateTime))
		}
	}

	if dvRow != nil {
		pageStack.Append(compton.FICenter(s, dvRow))
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

	osSymbol := compton.Sparkle
	if smb, ok := compton_data.OperatingSystemSymbols[os]; ok {
		osSymbol = smb
	}

	osString := ""
	switch os {
	case vangogh_integration.AnyOperatingSystem:
		osString = "Goodies"
	default:
		osString = os.String()
	}

	fmtOsBadge := compton.FormattedBadge{
		Title: osString,
		Icon:  osSymbol,
		Color: color.RepForeground,
	}

	return compton.SectionDivider(r, fmtOsBadge)
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
	if dv.totalEstimatedBytes > 0 {
		fr.PropVal("Size", vangogh_integration.FormatBytes(dv.totalEstimatedBytes))
	}

	var dvStatus string
	var dvSymbol compton.Symbol
	dvColor := color.RepGray

	if dv.downloadType == vangogh_integration.Installer || dv.downloadType == vangogh_integration.DLC {

		switch dv.validationStatus {
		case vangogh_integration.ValidationStatusUnknown:
			dvStatus = dv.downloadStatus.HumanReadableString()
			dvSymbol = compton_data.DownloadStatusSymbols[dv.downloadStatus]
		default:
			dvStatus = dv.validationStatus.HumanReadableString()
			dvColor = compton_data.ValidationStatusColors[dv.validationStatus]
			dvSymbol = compton_data.ValidationStatusSymbols[dv.validationStatus]
		}

	} else {
		dvStatus = dv.downloadStatus.HumanReadableString()
	}

	var progressEstimatedBytes int64

	if dv.downloadStatus == vangogh_integration.DownloadStatusDownloading && dv.downloadedEstimatedBytes > 0 {
		progressEstimatedBytes = dv.downloadedEstimatedBytes
	}

	if dv.validationStatus == vangogh_integration.ValidationStatusValidating && dv.validatedEstimatedBytes > 0 {
		progressEstimatedBytes = dv.validatedEstimatedBytes
	}

	if progressEstimatedBytes > 0 && dv.totalEstimatedBytes > 0 {
		progress := 100 * float64(progressEstimatedBytes) / float64(dv.totalEstimatedBytes)
		progressStr := strconv.FormatFloat(progress, 'f', 0, 64)
		dvStatus += ", " + progressStr + "% Done"
	}

	fmtDownloadValidationBadge := compton.FormattedBadge{
		Title: dvStatus,
		Icon:  dvSymbol,
		Color: dvColor,
	}

	var badges []compton.FormattedBadge
	badges = append(badges, fmtDownloadValidationBadge)

	if dv.generatedChecksum {
		generatedChecksumBadge := compton.FormattedBadge{
			Title: "Generated Checksum",
			Color: color.Yellow,
		}
		badges = append(badges, generatedChecksumBadge)
	}

	fr.Elements(compton.Badges(r, badges...))

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
	q.Set("download-type", dl.DownloadType.String())
	q.Set("manual-url", dl.ManualUrl)

	linkContainer := compton.FlexItems(r, direction.Column).RowGap(size.Small)

	var link compton.Element

	switch dl.DownloadType {
	case vangogh_integration.Extra:
		if dl.Info > 0 && dl.EstimatedBytes > 0 {
			link = compton.A("/files?" + q.Encode())
		} else {
			link = compton.Content()
		}
	default:
		link = compton.A("/files?" + q.Encode())
	}

	link.AddClass("download", dl.DownloadType.String())
	linkContainer.Append(link)

	linkColumn := compton.FlexItems(r, direction.Column).RowGap(size.Small)

	var name string
	switch dl.DownloadType {
	case vangogh_integration.DLC:
		name = dl.ProductTitle
	default:
		name = dl.Name
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

	dvs := vangogh_integration.NewManualUrlDvs(dl.ManualUrl, rdx)

	var manualUrlStatus string
	manualUrlStatusSymbol := compton.NoSymbol
	manualUrlStatusColor := color.RepGray

	if dl.DownloadType == vangogh_integration.Installer || dl.DownloadType == vangogh_integration.DLC {

		manualUrlStatus = dvs.HumanReadableString()
		manualUrlValidationStatus := dvs.ValidationStatus()

		switch manualUrlValidationStatus {
		case vangogh_integration.ValidationStatusUnknown:
			manualUrlStatusSymbol = compton_data.DownloadStatusSymbols[dvs.DownloadStatus()]
		default:
			manualUrlStatusColor = compton_data.ValidationStatusColors[manualUrlValidationStatus]
			manualUrlStatusSymbol = compton_data.ValidationStatusSymbols[manualUrlValidationStatus]
		}
	} else {
		manualUrlStatus = dvs.DownloadStatus().HumanReadableString()
	}

	manualUrlStatusBadge := compton.FormattedBadge{
		Title: manualUrlStatus,
		Icon:  manualUrlStatusSymbol,
		Color: manualUrlStatusColor,
	}

	var badges []compton.FormattedBadge
	badges = append(badges, manualUrlStatusBadge)

	if rdx.HasKey(vangogh_integration.ManualUrlGeneratedChecksumProperty, dl.ManualUrl) {
		generatedChecksumBadge := compton.FormattedBadge{
			Title: "Generated Checksum",
			Color: color.Yellow,
		}
		badges = append(badges, generatedChecksumBadge)
	}

	linkColumn.Append(compton.Badges(r, badges...))

	link.Append(linkColumn)

	bottomRow := compton.FlexItems(r, direction.Row).ColumnGap(size.Small).AlignItems(align.Center)

	copyManualUrlToClipboard := compton.CopyToClipboard(r,
		compton.Fspan(r, "Copy Manual URL").FontSize(size.XSmall).ForegroundColor(color.Blue).FontWeight(font_weight.Bolder),
		compton.Fspan(r, "Copied!").FontSize(size.XSmall).ForegroundColor(color.Green),
		compton.Fspan(r, "Error").FontSize(size.XSmall).ForegroundColor(color.Red),
		dl.ManualUrl)
	bottomRow.Append(copyManualUrlToClipboard)

	if dl.Type != "" {
		typeFr := compton.Frow(r).FontSize(size.XSmall)
		typeFr.PropVal("Type", dl.Type)
		bottomRow.Append(typeFr)
	}

	if dl.EstimatedBytes > 0 {
		sizeFr := compton.Frow(r).FontSize(size.XSmall)
		sizeFr.PropVal("Size", vangogh_integration.FormatBytes(dl.EstimatedBytes))
		bottomRow.Append(sizeFr)
	}

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

		dvs := vangogh_integration.NewManualUrlDvs(dl.ManualUrl, rdx)

		dv := &DownloadVariant{
			downloadType:        dl.DownloadType,
			version:             dl.Version,
			langCode:            dl.LanguageCode,
			totalEstimatedBytes: dl.EstimatedBytes,
			validationStatus:    dvs.ValidationStatus(),
			generatedChecksum:   rdx.HasKey(vangogh_integration.ManualUrlGeneratedChecksumProperty, dl.ManualUrl),
			downloadStatus:      dvs.DownloadStatus(),
		}

		if dv.downloadStatus == vangogh_integration.DownloadStatusDownloaded {
			dv.downloadedEstimatedBytes += dl.EstimatedBytes
		}

		if dv.downloadStatus == vangogh_integration.DownloadStatusValidated && dv.validationStatus.IsValidated() {
			dv.validatedEstimatedBytes += dl.EstimatedBytes
		}

		if edv := getDownloadVariant(variants, dv); edv == nil {
			variants = append(variants, dv)
		} else {
			edv.totalEstimatedBytes += dl.EstimatedBytes
			// use the "worst" validation result, worse = larger value
			if edv.validationStatus < dvs.ValidationStatus() {
				edv.validationStatus = dvs.ValidationStatus()
			}
			// use the "worst" manual url status, worse = larger value
			if edv.downloadStatus < dvs.DownloadStatus() {
				edv.downloadStatus = dvs.DownloadStatus()
			}

			edv.generatedChecksum = edv.generatedChecksum || rdx.HasKey(vangogh_integration.ManualUrlGeneratedChecksumProperty, dl.ManualUrl)

			if edv.downloadStatus == vangogh_integration.DownloadStatusDownloaded {
				edv.downloadedEstimatedBytes += dl.EstimatedBytes
			}
			if edv.downloadStatus == vangogh_integration.DownloadStatusValidated && dv.validationStatus.IsValidated() {
				edv.validatedEstimatedBytes += dl.EstimatedBytes
			}
		}

	}
	return variants
}

func filterDownloads(os vangogh_integration.OperatingSystem, dls vangogh_integration.DownloadsList, productTitle string, dv *DownloadVariant) []vangogh_integration.Download {
	downloads := make([]vangogh_integration.Download, 0)
	for _, dl := range dls {
		if dl.OS != os ||
			dl.DownloadType != dv.downloadType ||
			dv.version != dl.Version ||
			dv.langCode != dl.LanguageCode ||
			productTitle != dl.ProductTitle {
			continue
		}
		downloads = append(downloads, dl)
	}
	return downloads
}
