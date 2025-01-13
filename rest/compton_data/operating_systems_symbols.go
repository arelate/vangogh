package compton_data

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton"
)

var OperatingSystemSymbols = map[vangogh_integration.OperatingSystem]compton.Symbol{
	vangogh_integration.Windows: compton.Windows,
	vangogh_integration.MacOS:   compton.MacOS,
	vangogh_integration.Linux:   compton.Linux,
}
