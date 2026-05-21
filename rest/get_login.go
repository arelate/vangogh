package rest

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/nod"
)

func GetLogin(w http.ResponseWriter, r *http.Request) {

	var p compton.PageElement

	err := sb.MustHaveUsers()
	switch err {
	case nil:
		p = compton_pages.Login("/auth")
	default:
		p = compton_pages.NoUsers()
	}

	if err = p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
