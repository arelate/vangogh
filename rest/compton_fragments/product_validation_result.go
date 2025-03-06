package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
)

var ValidationResultsColors = map[vangogh_integration.ValidationResult]color.Color{
	vangogh_integration.ValidationResultUnknown:        color.Gray,
	vangogh_integration.ValidatedSuccessfully:          color.Green,
	vangogh_integration.ValidatedWithGeneratedChecksum: color.Green,
	vangogh_integration.ValidatedUnresolvedManualUrl:   color.Teal,
	vangogh_integration.ValidatedMissingLocalFile:      color.Teal,
	vangogh_integration.ValidatedMissingChecksum:       color.Teal,
	vangogh_integration.ValidationError:                color.Orange,
	vangogh_integration.ValidatedChecksumMismatch:      color.Red,
}

func ProductValidationResult(id string, rdx redux.Readable) (string, color.Color) {

	if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {

		pvr := vangogh_integration.ParseValidationResult(pvrs)

		//prve := compton.Fspan(r, pvr.HumanReadableString()).
		//	FontSize(size.Small).
		//	FontWeight(font_weight.Normal).
		//	PaddingInline(size.XSmall).
		//	PaddingBlock(size.XXSmall).
		//	BorderRadius(size.XXSmall)
		//
		//pvrc = ValidationResultsColors[pvr]

		return pvr.HumanReadableString(), ValidationResultsColors[pvr]
	}

	return "", color.Transparent
}
