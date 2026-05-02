package cli

import (
	"net/url"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
)

func ImportCookiesHandler(u *url.URL) error {

	ica := nod.Begin("importing cookies...")
	defer ica.Done()

	cookieStr := u.Query().Get("cookies")

	absCookiesFilename := vangogh_integration.AbsCookiePath()

	if err := coost.Import(cookieStr, gog_integration.HostUrl(), absCookiesFilename); err != nil {
		return err
	}

	hc, err := gogAuthHttpClient()
	if err != nil {
		return err
	}

	return gog_integration.IsLoggedIn(hc)
}
