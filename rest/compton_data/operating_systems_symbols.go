package compton_data

import (
	"github.com/arelate/vangogh_local_data"
	"github.com/boggydigital/compton"
)

var OperatingSystemSymbols = map[vangogh_local_data.OperatingSystem]compton.Symbol{
	vangogh_local_data.Windows: compton.Windows,
	vangogh_local_data.MacOS:   compton.MacOS,
	vangogh_local_data.Linux:   compton.Linux,
}
