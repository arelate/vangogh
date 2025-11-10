package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
)

var ValidationStatusColors = map[vangogh_integration.ValidationStatus]color.Color{
	vangogh_integration.ValidationStatusQueued:              color.RepGray,
	vangogh_integration.ValidationStatusValidating:          color.RepGray,
	vangogh_integration.ValidationStatusSuccess:             color.Green,
	vangogh_integration.ValidationStatusUnknown:             color.RepGray,
	vangogh_integration.ValidationStatusUnresolvedManualUrl: color.Orange,
	vangogh_integration.ValidationStatusMissingLocalFile:    color.Orange,
	vangogh_integration.ValidationStatusMissingChecksum:     color.Orange,
	vangogh_integration.ValidationStatusError:               color.Red,
	vangogh_integration.ValidationStatusChecksumMismatch:    color.Red,
}

var ValidationStatusSymbols = map[vangogh_integration.ValidationStatus]compton.Symbol{
	vangogh_integration.ValidationStatusSuccess:             compton.HexagonSparkling,
	vangogh_integration.ValidationStatusUnknown:             compton.HexagonDiagonalLines,
	vangogh_integration.ValidationStatusQueued:              compton.HexagonClockArrows,
	vangogh_integration.ValidationStatusValidating:          compton.HexagonDownwardArrow,
	vangogh_integration.ValidationStatusMissingChecksum:     compton.Triangle,
	vangogh_integration.ValidationStatusUnresolvedManualUrl: compton.Triangle,
	vangogh_integration.ValidationStatusMissingLocalFile:    compton.Triangle,
	vangogh_integration.ValidationStatusError:               compton.Cross,
	vangogh_integration.ValidationStatusChecksumMismatch:    compton.Cross,
}

var DownloadStatusSymbols = map[vangogh_integration.DownloadStatus]compton.Symbol{
	vangogh_integration.DownloadStatusDownloaded:  compton.CircleCompactDisk,
	vangogh_integration.DownloadStatusUnknown:     compton.CircleDashed,
	vangogh_integration.DownloadStatusQueued:      compton.CircleClockArrows,
	vangogh_integration.DownloadStatusDownloading: compton.CircleDownwardArrow,
	vangogh_integration.DownloadStatusInterrupted: compton.Cross,
}

var DownloadTypesSymbols = map[vangogh_integration.DownloadType]compton.Symbol{
	vangogh_integration.Installer: compton.CircleCompactDisk,
	vangogh_integration.DLC:       compton.ItemPlus,
	vangogh_integration.Extra:     compton.Sparkle,
}
