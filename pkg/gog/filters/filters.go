package filters

import (
	"golang.org/x/net/html"
	"strings"

	vhtml "github.com/boggydigital/vangogh/pkg/html"
)

func InputLoginToken(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "input" &&
		vhtml.AttrVal(n, "name") == "login[_token]"
}

func InputSecondStepAuthToken(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "input" &&
		vhtml.AttrVal(n, "name") == "second_step_authentication[_token]"
}

func ScriptReCaptcha(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "script" &&
		strings.HasPrefix(
			vhtml.AttrVal(n, "src"),
			"https://www.recaptcha.net/recaptcha/api.js")
}
