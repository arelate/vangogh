package compton_fragments

import "github.com/arelate/vangogh/rest/compton_data"

func PageTitle(current string) string {
	if current != "" {
		return current + " - " + compton_data.AppTitle
	}
	return compton_data.AppTitle
}
