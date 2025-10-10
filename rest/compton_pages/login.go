package compton_pages

import (
	"net/http"

	"github.com/arelate/vangogh/rest/compton_fragments"
	"github.com/boggydigital/author"
	"github.com/boggydigital/compton"
	"github.com/boggydigital/compton/consts/align"
	"github.com/boggydigital/compton/consts/color"
	"github.com/boggydigital/compton/consts/direction"
	"github.com/boggydigital/compton/consts/size"
	"golang.org/x/net/html/atom"
)

const (
	pageTitle          = "Restricted area"
	pageSubTitle       = "Authorized personnel only"
	usernameLabelTitle = "Username"
	passwordLabelTitle = "Password"
	submitButtonTitle  = "Login"
)

func Login(authPath string) compton.PageElement {

	p := compton.Page(pageTitle)
	pageStack := compton.FlexItems(p, direction.Column).RowGap(size.Large)
	p.Append(pageStack)

	headingsStack := compton.FlexItems(p, direction.Column).RowGap(size.Small)
	titleHeading := compton.H2Text(pageTitle)
	subTitleHeading := compton.H3()
	subTitleHeading.Append(compton.Fspan(p, pageSubTitle).ForegroundColor(color.Gray))

	headingsStack.Append(titleHeading, subTitleHeading)

	pageStack.Append(compton.Div(), compton.FICenter(p, headingsStack))

	form := compton.Form(authPath, http.MethodPost)
	pageStack.Append(form)

	formStack := compton.FlexItems(p, direction.Column).RowGap(size.Normal).AlignContent(align.Center)
	form.Append(formStack)

	tiUsername := compton.TIText(p, usernameLabelTitle, author.UsernameParam).Width(size.XXXLarge)
	tiPassword := compton.TIPassword(p, passwordLabelTitle, author.PasswordParam).Width(size.XXXLarge)

	if input := tiUsername.GetFirstElementByTagName(atom.Input); input != nil {
		input.SetAttribute("autofocus", "")
		input.SetAttribute("autocapitalize", "none")
	}

	submitNavLink := compton.NavLinks(p)
	submitNavLink.AppendSubmitLink(p, &compton.NavTarget{
		Href:  "#",
		Title: submitButtonTitle,
	})

	formStack.Append(tiUsername, tiPassword, submitNavLink)

	pageStack.Append(compton.FICenter(p, compton_fragments.GitHubLink(p)))

	return p
}
