package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
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

func ProductValidationResult(r compton.Registrar, id string, rdx redux.Readable) compton.Element {
	pvrc := color.Gray
	if pvrs, ok := rdx.GetLastVal(vangogh_integration.ProductValidationResultProperty, id); ok {
		pvr := vangogh_integration.ParseValidationResult(pvrs)
		pvrc = ValidationResultsColors[pvr]

		return compton.SvgUse(r, compton.Circle).ForegroundColor(pvrc)
	}

	return nil
}
