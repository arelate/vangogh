// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package matches

import (
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"github.com/boggydigital/vangogh/internal/html/attr"
	"golang.org/x/net/html"
	"strings"
)

func InputLoginToken(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "input" &&
		attr.Val(n, "name") == "login[_token]"
}

func InputSecondStepAuthToken(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "input" &&
		attr.Val(n, "name") == "second_step_authentication[_token]"
}

func ScriptReCaptcha(n *html.Node) bool {
	return n != nil &&
		n.Type == html.ElementNode &&
		n.Data == "script" &&
		strings.HasPrefix(attr.Val(n, "src"), urls.ReCaptchaURL)
}
