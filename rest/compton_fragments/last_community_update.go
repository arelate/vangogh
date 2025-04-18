package compton_fragments

import (
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/redux"
	"time"
)

func LastCommunityUpdate(id string, rdx redux.Readable) (string, color.Color) {

	if lcus, ok := rdx.GetLastVal(vangogh_integration.SteamLastCommunityUpdateProperty, id); ok && lcus != "" {
		if lcut, err := time.Parse(time.RFC3339, lcus); err == nil {

			c := color.Gray
			if lcut.After(time.Now().Add(-1 * time.Hour * 24 * 30)) {
				c = color.Green
			}

			return lcut.Format("Jan 2, '06"), c
		}
	}

	return "", color.Gray
}
