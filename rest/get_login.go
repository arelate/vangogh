package rest

import (
	"net/http"

	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/nod"
)

func GetLogin(w http.ResponseWriter, r *http.Request) {

	p := compton.Page("vangogh login")

	form := compton.Form("/login", http.MethodPost)
	p.Append(form)

	formStack := compton.FlexItems(p, direction.Column)
	form.Append(formStack)

	tiUsername := compton.TIText(p, "Username", author.UsernameParam)
	tiPassword := compton.TIPassword(p, "Password", author.PasswordParam)

	submitNavLink := compton.NavLinks(p)
	submitNavLink.AppendSubmitLink(p, &compton.NavTarget{
		Href:  "#",
		Title: "Login",
	})

	formStack.Append(tiUsername, tiPassword, submitNavLink)

	if err := p.WriteResponse(w); err != nil {
		http.Error(w, nod.Error(err).Error(), http.StatusInternalServerError)
	}

}
