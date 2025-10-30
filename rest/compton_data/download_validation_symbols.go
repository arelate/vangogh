package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/color"
)

var ValidationResultsColors = map[vangogh_integration.ValidationResult]color.Color{
	vangogh_integration.ValidationQueued:             color.RepGray,
	vangogh_integration.ValidationValidating:         color.RepGray,
	vangogh_integration.ValidatedSuccessfully:        color.Green,
	vangogh_integration.ValidationResultUnknown:      color.RepGray,
	vangogh_integration.ValidatedUnresolvedManualUrl: color.Orange,
	vangogh_integration.ValidatedMissingLocalFile:    color.Orange,
	vangogh_integration.ValidatedMissingChecksum:     color.Orange,
	vangogh_integration.ValidationError:              color.Red,
	vangogh_integration.ValidatedChecksumMismatch:    color.Red,
}

var ValidationResultsSymbols = map[vangogh_integration.ValidationResult]compton.Symbol{
	vangogh_integration.ValidatedSuccessfully:        compton.Hexagon,
	vangogh_integration.ValidationResultUnknown:      compton.DashedHexagon,
	vangogh_integration.ValidationQueued:             compton.HexagonClockFace,
	vangogh_integration.ValidationValidating:         compton.CyclingHexagon,
	vangogh_integration.ValidatedMissingChecksum:     compton.Triangle,
	vangogh_integration.ValidatedUnresolvedManualUrl: compton.Triangle,
	vangogh_integration.ValidatedMissingLocalFile:    compton.Triangle,
	vangogh_integration.ValidationError:              compton.Cross,
	vangogh_integration.ValidatedChecksumMismatch:    compton.Cross,
}

var ManualUrlStatusSymbols = map[vangogh_integration.ManualUrlStatus]compton.Symbol{
	vangogh_integration.ManualUrlDownloaded:          compton.CompactDisk,
	vangogh_integration.ManualUrlStatusUnknown:       compton.DashedCircle,
	vangogh_integration.ManualUrlQueued:              compton.ClockFace,
	vangogh_integration.ManualUrlDownloading:         compton.CyclingCircle,
	vangogh_integration.ManualUrlDownloadInterrupted: compton.Cross,
}

var DownloadTypesSymbols = map[vangogh_integration.DownloadType]compton.Symbol{
	vangogh_integration.Installer: compton.CompactDisk,
	vangogh_integration.DLC:       compton.ItemPlus,
	vangogh_integration.Extra:     compton.Sparkle,
}
