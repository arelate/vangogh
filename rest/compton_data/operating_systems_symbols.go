package compton_data

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton/elements/svg_use"
)

var OperatingSystemSymbols = map[vangogh_local_data.OperatingSystem]svg_use.Symbol{
	vangogh_local_data.Windows: svg_use.Windows,
	vangogh_local_data.MacOS:   svg_use.MacOS,
	vangogh_local_data.Linux:   svg_use.Linux,
}
