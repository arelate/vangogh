package compton_data

import (
	"github.com/boggydigital/compton"
)

const (
	AppNavUpdates = "Updates"
	AppNavSearch  = "Search"
)

var AppNavOrder = []string{AppNavUpdates, AppNavSearch}

var AppNavIcons = map[string]compton.Symbol{
	AppNavUpdates: compton.Sparkle,
	AppNavSearch:  compton.Search,
}

var AppNavLinks = map[string]string{
	AppNavUpdates: "/updates",
	AppNavSearch:  "/search",
}
