package rest

import (
	"errors"
	"net/http"

	"github.com/arelate/vangogh/rest/compton_pages"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/nod"
)

func GetLogin(w http.ResponseWriter, r *http.Request) {

	var p compton.PageElement

	if err := sb.MustHaveUsers(); err == nil {
		p = compton_pages.Login("/auth")
	} else if errors.Is(err, author.ErrNoUsers) {
		p = compton_pages.NoUsers()
	} else {
		p = compton_pages.Error(err)
	}

	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
