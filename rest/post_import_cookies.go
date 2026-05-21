package rest

import (
	"errors"
	"net/http"
	"os"

	"github.com/arelate/southern_light/gog_integration"
	"github.com/arelate/southern_light/vangogh_integration"
	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/coost"
	"github.com/boggydigital/nod"
)

const candidateExt = ".candidate"

func PostImportCookies(w http.ResponseWriter, r *http.Request) {

	// POST /import-cookies

	if err := r.ParseForm(); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusBadRequest)
		return
	}

	var p compton.PageElement

	var importCookies string
	if importCookiesValues := r.Form["import-cookies"]; len(importCookiesValues) > 0 {
		importCookies = importCookiesValues[0]
	}

	if importCookies == "" {
		p = compton_pages.Error(errors.New("GOG.com cookie cannot be empty"))
	} else {
		absCandidateCookiesPath := vangogh_integration.AbsCookiesPath() + candidateExt
		if err := coost.Import(importCookies, gog_integration.HostUrl(), absCandidateCookiesPath); err == nil {
			var jar http.CookieJar
			if jar, err = coost.Read(gog_integration.HostUrl(), absCandidateCookiesPath); err == nil {
				hc := http.DefaultClient
				hc.Jar = jar
				if err = gog_integration.IsLoggedIn(hc); err == nil {

					absCookiesPath := vangogh_integration.AbsCookiesPath()
					if err = os.Rename(absCandidateCookiesPath, absCookiesPath); err == nil {
						// success

						w.Header().Set("Location", "/")
						w.WriteHeader(http.StatusSeeOther)

						return

					} else {
						p = compton_pages.Error(errors.New("invalid cookies, error authorizing with GOG.com"))
					}
				} else if errors.Is(err, gog_integration.ErrNotLoggedIn) {
					p = compton_pages.Error(errors.New("invalid cookies, error authorizing with GOG.com"))
				}
			} else {
				p = compton_pages.Error(err)
			}
		} else {
			p = compton_pages.Error(err)
		}
	}

	if p != nil {
		if err := p.WriteResponse(w); err != nil {
			http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
		}
	}
}
