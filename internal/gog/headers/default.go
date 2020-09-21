// Copyright (c) 2020. All rights reserved.
// Use of this source code is governed by an MIT-style license that can be
// found in the LICENSE file.

package headers

import "net/http"

func Default(req *http.Request, host string) {
	const (
		acceptHeader         = "text/html"
		acceptLanguageHeader = "en-us"
		connectionHeader     = "keep-alive"
		userAgentHeader      = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_6) " +
			"AppleWebKit/537.36 (KHTML, like Gecko) " +
			"Chrome/84.0.4147.105 " +
			"Safari/537.36 " +
			"Edg/84.0.522.52" // Microsoft Edge 84 UA string
	)
	req.Host = host
	req.Header.Set("Accept", acceptHeader)
	req.Header.Set("Accept-Language", acceptLanguageHeader)
	req.Header.Set("Connection", connectionHeader)
	req.Header.Set("User-Agent", userAgentHeader)
}
