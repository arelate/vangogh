// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package headers

import (
	"github.com/boggydigital/vangogh/internal/gog/urls"
	"net/http"
	"net/url"
)

func Form(req *http.Request, referer *url.URL) {
	const (
		urlEncoded = "application/x-www-form-urlencoded"
	)

	req.Header.Set("Origin", urls.LoginHost)
	req.Header.Set("Referer", referer.String())
	req.Header.Set("Content-Type", urlEncoded)
}
