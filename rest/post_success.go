package rest

import (
	"net/http"

	"github.com/boggydigital/compton"
	"github.com/boggydigital/nod"
)

func PostSuccess(w http.ResponseWriter, r *http.Request) {

	p := compton.Page("vangogh - successful login")

	p.Append(compton.AText("Click to go the homepage", "/"))

	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}
}
