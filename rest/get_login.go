package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/nod"
)

func GetLogin(w http.ResponseWriter, r *http.Request) {

	p := compton_pages.Login("/auth")

	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
